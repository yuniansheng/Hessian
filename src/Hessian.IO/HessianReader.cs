using System;
using System.Runtime;
using System.Text;
using System.Globalization;
using System.Diagnostics.Contracts;
using System.Security;
using System.IO;

namespace Hessian.IO
{
    public class HessianReader : IDisposable
    {
        private const int MaxCharBytesSize = 128;

        private Stream m_stream;
        private byte[] m_buffer;
        private Decoder m_decoder;
        private byte[] m_charBytes;
        private char[] m_singleChar;
        private char[] m_charBuffer;
        private int m_maxCharsSize;  // From MaxCharBytesSize & Encoding

        // Performance optimization for Read() w/ Unicode.  Speeds us up by ~40% 
        private bool m_2BytesPerChar;
        private bool m_isMemoryStream; // "do we sit on MemoryStream?" for Read/ReadInt32 perf
        private bool m_leaveOpen;

        public HessianReader(Stream input) : this(input, true)
        {
        }

        public HessianReader(Stream input, bool leaveOpen)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (!input.CanRead)
                throw new ArgumentException("Argument_StreamNotReadable");
            Contract.EndContractBlock();
            Encoding encoding = new UTF8Encoding(false, true);
            m_stream = input;
            m_decoder = encoding.GetDecoder();
            m_maxCharsSize = encoding.GetMaxCharCount(MaxCharBytesSize);
            int minBufferSize = encoding.GetMaxByteCount(1);  // max bytes per one char
            if (minBufferSize < 16)
                minBufferSize = 16;
            m_buffer = new byte[minBufferSize];
            // m_charBuffer and m_charBytes will be left null.

            // For Encodings that always use 2 bytes per char (or more), 
            // special case them here to make Read() & Peek() faster.
            m_2BytesPerChar = encoding is UnicodeEncoding;
            // check if BinaryReader is based on MemoryStream, and keep this for it's life
            // we cannot use "as" operator, since derived classes are not allowed
            m_isMemoryStream = (m_stream.GetType() == typeof(MemoryStream));
            m_leaveOpen = leaveOpen;
        }

        public virtual Stream BaseStream
        {
            get
            {
                return m_stream;
            }
        }

        public virtual void Close()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stream copyOfStream = m_stream;
                m_stream = null;
                if (copyOfStream != null && !m_leaveOpen)
                    copyOfStream.Close();
            }
            m_stream = null;
            m_buffer = null;
            m_decoder = null;
            m_charBytes = null;
            m_singleChar = null;
            m_charBuffer = null;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public virtual int PeekChar()
        {
            Contract.Ensures(Contract.Result<int>() >= -1);

            if (m_stream == null) throw Error.GetFileNotOpen();

            if (!m_stream.CanSeek)
                return -1;
            long origPos = m_stream.Position;
            int ch = Read();
            m_stream.Position = origPos;
            return ch;
        }

        public virtual int Read()
        {
            Contract.Ensures(Contract.Result<int>() >= -1);

            if (m_stream == null)
            {
                throw Error.GetFileNotOpen();
            }
            return InternalReadOneChar();
        }

        public virtual bool ReadBoolean()
        {
            FillBuffer(1);
            return (m_buffer[0] != 0);
        }

        public virtual byte ReadByte()
        {
            // Inlined to avoid some method call overhead with FillBuffer.
            if (m_stream == null) throw Error.GetFileNotOpen();

            int b = m_stream.ReadByte();
            if (b == -1)
                throw Error.GetEndOfFile();
            return (byte)b;
        }

        public virtual sbyte ReadSByte()
        {
            FillBuffer(1);
            return (sbyte)(m_buffer[0]);
        }

        public virtual char ReadChar()
        {
            int value = Read();
            if (value == -1)
            {
                throw Error.GetEndOfFile();
            }
            return (char)value;
        }

        public virtual short ReadInt16()
        {
            FillBuffer(2);
            return (short)(m_buffer[1] | m_buffer[0] << 8);
        }

        public virtual ushort ReadUInt16()
        {
            FillBuffer(2);
            return (ushort)(m_buffer[1] | m_buffer[0] << 8);
        }

        public virtual int ReadInt32()
        {
            FillBuffer(4);
            return (int)(m_buffer[3] | m_buffer[2] << 8 | m_buffer[1] << 16 | m_buffer[0] << 24);
        }

        public virtual uint ReadUInt32()
        {
            FillBuffer(4);
            return (uint)(m_buffer[3] | m_buffer[2] << 8 | m_buffer[1] << 16 | m_buffer[0] << 24);
        }

        public virtual long ReadInt64()
        {
            FillBuffer(8);
            uint lo = (uint)(m_buffer[7] | m_buffer[6] << 8 |
                             m_buffer[5] << 16 | m_buffer[4] << 24);
            uint hi = (uint)(m_buffer[3] | m_buffer[2] << 8 |
                             m_buffer[1] << 16 | m_buffer[0] << 24);
            return (long)((ulong)hi) << 32 | lo;
        }

        public virtual ulong ReadUInt64()
        {
            FillBuffer(8);
            uint lo = (uint)(m_buffer[7] | m_buffer[6] << 8 |
                             m_buffer[5] << 16 | m_buffer[4] << 24);
            uint hi = (uint)(m_buffer[3] | m_buffer[2] << 8 |
                             m_buffer[1] << 16 | m_buffer[0] << 24);
            return ((ulong)hi) << 32 | lo;
        }

        [System.Security.SecuritySafeCritical]  // auto-generated
        public virtual unsafe float ReadSingle()
        {
            FillBuffer(4);
            uint tmpBuffer = (uint)(m_buffer[3] | m_buffer[2] << 8 | m_buffer[1] << 16 | m_buffer[0] << 24);
            return *((float*)&tmpBuffer);
        }

        [System.Security.SecuritySafeCritical]  // auto-generated
        public virtual unsafe double ReadDouble()
        {
            FillBuffer(8);
            uint lo = (uint)(m_buffer[7] | m_buffer[6] << 8 |
                m_buffer[5] << 16 | m_buffer[4] << 24);
            uint hi = (uint)(m_buffer[3] | m_buffer[2] << 8 |
                m_buffer[1] << 16 | m_buffer[0] << 24);

            ulong tmpBuffer = ((ulong)hi) << 32 | lo;
            return *((double*)&tmpBuffer);
        }

        public virtual String ReadString(int stringLength)
        {
            Contract.Ensures(Contract.Result<String>() != null);

            if (m_stream == null)
                throw Error.GetFileNotOpen();

            int currPos = 0;
            int n;
            int readLength;
            int charsRead;

            if (stringLength < 0)
            {
                throw new IOException("IO.IO_InvalidStringLen_Len", stringLength);
            }

            if (stringLength == 0)
            {
                return String.Empty;
            }

            if (m_charBytes == null)
            {
                m_charBytes = new byte[MaxCharBytesSize];
            }

            if (m_charBuffer == null)
            {
                m_charBuffer = new char[m_maxCharsSize];
            }

            StringBuilder sb = null;
            do
            {
                readLength = ((stringLength - currPos) > MaxCharBytesSize) ? MaxCharBytesSize : (stringLength - currPos);

                n = m_stream.Read(m_charBytes, 0, readLength);
                if (n == 0)
                {
                    throw Error.GetEndOfFile();
                }

                charsRead = m_decoder.GetChars(m_charBytes, 0, n, m_charBuffer, 0);

                if (currPos == 0 && n == stringLength)
                    return new String(m_charBuffer, 0, charsRead);

                if (sb == null)
                    sb = new StringBuilder(stringLength); // Actual string length in chars may be smaller.
                sb.Append(m_charBuffer, 0, charsRead);
                currPos += n;

            } while (currPos < stringLength);

            return sb.ToString();
        }

        [SecuritySafeCritical]
        public virtual int Read(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer", ("ArgumentNull_Buffer"));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_NeedNonNegNum");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_NeedNonNegNum");
            }
            if (buffer.Length - index < count)
            {
                throw new ArgumentException("Argument_InvalidOffLen");
            }
            Contract.Ensures(Contract.Result<int>() >= 0);
            Contract.Ensures(Contract.Result<int>() <= count);
            Contract.EndContractBlock();
            if (m_stream == null)
                throw Error.GetFileNotOpen();

            // SafeCritical: index and count have already been verified to be a valid range for the buffer
            return InternalReadChars(buffer, index, count);
        }

        [SecurityCritical]
        private int InternalReadChars(char[] buffer, int index, int count)
        {
            Contract.Requires(buffer != null);
            Contract.Requires(index >= 0 && count >= 0);
            Contract.Assert(m_stream != null);

            int numBytes = 0;
            int charsRemaining = count;

            if (m_charBytes == null)
            {
                m_charBytes = new byte[MaxCharBytesSize];
            }

            while (charsRemaining > 0)
            {
                int charsRead = 0;
                // We really want to know what the minimum number of bytes per char
                // is for our encoding.  Otherwise for UnicodeEncoding we'd have to
                // do ~1+log(n) reads to read n characters.
                numBytes = charsRemaining;

                if (m_2BytesPerChar)
                    numBytes <<= 1;
                if (numBytes > MaxCharBytesSize)
                    numBytes = MaxCharBytesSize;

                int position = 0;
                byte[] byteBuffer = null;
                numBytes = m_stream.Read(m_charBytes, 0, numBytes);
                byteBuffer = m_charBytes;

                if (numBytes == 0)
                {
                    return (count - charsRemaining);
                }

                Contract.Assert(byteBuffer != null, "expected byteBuffer to be non-null");

                checked
                {

                    if (position < 0 || numBytes < 0 || position + numBytes > byteBuffer.Length)
                    {
                        throw new ArgumentOutOfRangeException("byteCount");
                    }

                    if (index < 0 || charsRemaining < 0 || index + charsRemaining > buffer.Length)
                    {
                        throw new ArgumentOutOfRangeException("charsRemaining");
                    }

                    unsafe
                    {
                        fixed (byte* pBytes = byteBuffer)
                        {
                            fixed (char* pChars = buffer)
                            {
                                charsRead = m_decoder.GetChars(pBytes + position, numBytes, pChars + index, charsRemaining, false);
                            }
                        }
                    }
                }

                charsRemaining -= charsRead;
                index += charsRead;
            }

            // this should never fail
            Contract.Assert(charsRemaining >= 0, "We read too many characters.");

            // we may have read fewer than the number of characters requested if end of stream reached 
            // or if the encoding makes the char count too big for the buffer (e.g. fallback sequence)
            return (count - charsRemaining);
        }

        private int InternalReadOneChar()
        {
            // I know having a separate InternalReadOneChar method seems a little 
            // redundant, but this makes a scenario like the security parser code
            // 20% faster, in addition to the optimizations for UnicodeEncoding I
            // put in InternalReadChars.   
            int charsRead = 0;
            int numBytes = 0;
            long posSav = posSav = 0;

            if (m_stream.CanSeek)
                posSav = m_stream.Position;

            if (m_charBytes == null)
            {
                m_charBytes = new byte[MaxCharBytesSize]; //
            }
            if (m_singleChar == null)
            {
                m_singleChar = new char[1];
            }

            while (charsRead == 0)
            {
                // We really want to know what the minimum number of bytes per char
                // is for our encoding.  Otherwise for UnicodeEncoding we'd have to
                // do ~1+log(n) reads to read n characters.
                // Assume 1 byte can be 1 char unless m_2BytesPerChar is true.
                numBytes = m_2BytesPerChar ? 2 : 1;

                int r = m_stream.ReadByte();
                m_charBytes[0] = (byte)r;
                if (r == -1)
                    numBytes = 0;
                if (numBytes == 2)
                {
                    r = m_stream.ReadByte();
                    m_charBytes[1] = (byte)r;
                    if (r == -1)
                        numBytes = 1;
                }

                if (numBytes == 0)
                {
                    // Console.WriteLine("Found no bytes.  We're outta here.");
                    return -1;
                }

                Contract.Assert(numBytes == 1 || numBytes == 2, "BinaryReader::InternalReadOneChar assumes it's reading one or 2 bytes only.");

                try
                {

                    charsRead = m_decoder.GetChars(m_charBytes, 0, numBytes, m_singleChar, 0);
                }
                catch
                {
                    // Handle surrogate char 

                    if (m_stream.CanSeek)
                        m_stream.Seek((posSav - m_stream.Position), SeekOrigin.Current);
                    // else - we can't do much here

                    throw;
                }

                Contract.Assert(charsRead < 2, "InternalReadOneChar - assuming we only got 0 or 1 char, not 2!");
                //                Console.WriteLine("That became: " + charsRead + " characters.");
            }
            if (charsRead == 0)
                return -1;
            return m_singleChar[0];
        }

        [SecuritySafeCritical]
        public virtual char[] ReadChars(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", ("ArgumentOutOfRange_NeedNonNegNum"));
            }
            Contract.Ensures(Contract.Result<char[]>() != null);
            Contract.Ensures(Contract.Result<char[]>().Length <= count);
            Contract.EndContractBlock();
            if (m_stream == null)
            {
                throw Error.GetFileNotOpen();
            }

            if (count == 0)
            {
                return new char[0];
            }

            // SafeCritical: we own the chars buffer, and therefore can guarantee that the index and count are valid
            char[] chars = new char[count];
            int n = InternalReadChars(chars, 0, count);
            if (n != count)
            {
                char[] copy = new char[n];
                Buffer.BlockCopy(chars, 0, copy, 0, 2 * n); // sizeof(char)
                chars = copy;
            }

            return chars;
        }

        public virtual int Read(byte[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", ("ArgumentNull_Buffer"));
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", ("ArgumentOutOfRange_NeedNonNegNum"));
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", ("ArgumentOutOfRange_NeedNonNegNum"));
            if (buffer.Length - index < count)
                throw new ArgumentException(("Argument_InvalidOffLen"));
            Contract.Ensures(Contract.Result<int>() >= 0);
            Contract.Ensures(Contract.Result<int>() <= count);
            Contract.EndContractBlock();

            if (m_stream == null) throw Error.GetFileNotOpen();
            return m_stream.Read(buffer, index, count);
        }

        public virtual byte[] ReadBytes(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException("count", ("ArgumentOutOfRange_NeedNonNegNum"));
            Contract.Ensures(Contract.Result<byte[]>() != null);
            Contract.Ensures(Contract.Result<byte[]>().Length <= Contract.OldValue(count));
            Contract.EndContractBlock();
            if (m_stream == null) throw Error.GetFileNotOpen();

            if (count == 0)
            {
                return new byte[0];
            }

            byte[] result = new byte[count];

            int numRead = 0;
            do
            {
                int n = m_stream.Read(result, numRead, count);
                if (n == 0)
                    break;
                numRead += n;
                count -= n;
            } while (count > 0);

            if (numRead != result.Length)
            {
                // Trim array.  This should happen on EOF & possibly net streams.
                byte[] copy = new byte[numRead];
                Buffer.BlockCopy(result, 0, copy, 0, numRead);
                result = copy;
            }

            return result;
        }

        protected virtual void FillBuffer(int numBytes)
        {
            if (m_buffer != null && (numBytes < 0 || numBytes > m_buffer.Length))
            {
                throw new ArgumentOutOfRangeException("numBytes", ("ArgumentOutOfRange_BinaryReaderFillBuffer"));
            }
            int bytesRead = 0;
            int n = 0;

            if (m_stream == null) throw Error.GetFileNotOpen();

            // Need to find a good threshold for calling ReadByte() repeatedly
            // vs. calling Read(byte[], int, int) for both buffered & unbuffered
            // streams.
            if (numBytes == 1)
            {
                n = m_stream.ReadByte();
                if (n == -1)
                    throw Error.GetEndOfFile();
                m_buffer[0] = (byte)n;
                return;
            }

            do
            {
                n = m_stream.Read(m_buffer, bytesRead, numBytes - bytesRead);
                if (n == 0)
                {
                    throw Error.GetEndOfFile();
                }
                bytesRead += n;
            } while (bytesRead < numBytes);
        }
    }
}