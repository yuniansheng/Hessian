using System;
using System.Collections.Generic;
using System.Text;

namespace Hessian.IO
{
    public class ReferenceMap<T>
    {
        private List<T> _list = new List<T>();
        private Dictionary<T, int> _dict = new Dictionary<T, int>();

        ///// <summary>
        ///// add item to the map or return index of an already existed item
        ///// </summary>
        ///// <param name="value">the item</param>
        ///// <returns>if the item already existed return the index, or return -1</returns>

        /// <summary>
        /// add item to the map or return index of an already existed item
        /// </summary>
        /// <param name="value">the item</param>
        /// <returns>index: the item index;isNewItem: true for a new item</returns>
        public (int index, bool isNewItem) AddItem(T value)
        {
            if (_dict.TryGetValue(value, out int index))
            {
                return (index, false);
            }
            else
            {
                index = _list.Count;
                _dict.Add(value, index);
                _list.Add(value);
                return (index, true);
            }
        }

        public void Clear()
        {
            _list.Clear();
            _dict.Clear();
        }

        public T GetItem(int index)
        {
            return _list[index];
        }
    }
}
