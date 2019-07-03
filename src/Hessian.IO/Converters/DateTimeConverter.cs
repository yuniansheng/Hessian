using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class DateTimeConverter : HessianConverter
    {
        public DateTime BeginDate { get; set; } = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            var type = value.GetType();
            if (type != typeof(DateTime))
            {
                throw Exceptions.UnExpectedTypeException(type);
            }

            DateTime time = (DateTime)value;
            if (time.Kind == DateTimeKind.Unspecified)
            {
                time = new DateTime(time.Ticks, DateTimeKind.Utc);
            }
            else if (time.Kind == DateTimeKind.Local)
            {
                time = time.ToUniversalTime();
            }
            long timeStamp = (time.Ticks - BeginDate.Ticks) / 10000;

            if (timeStamp % 60000L == 0)
            {
                long minutes = timeStamp / 60000L;

                if ((minutes >> 31) == 0 || (minutes >> 31) == -1)
                {
                    writer.Write(Constants.BC_DATE_MINUTE);
                    writer.Write((int)minutes);
                    return;
                }
            }

            writer.Write(Constants.BC_DATE);
            writer.Write(timeStamp);
        }
    }
}
