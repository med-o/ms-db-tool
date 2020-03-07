using System;
using System.Collections.Generic;

namespace Database.Core.Schema
{
    public class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
        protected readonly Func<T, TKey> _keyExtractor;

        public KeyEqualityComparer(Func<T, TKey> keyExtractor)
        {
            _keyExtractor = keyExtractor;
        }

        public virtual bool Equals(T x, T y)
        {
            return _keyExtractor(x).Equals(_keyExtractor(y));
        }

        public int GetHashCode(T obj)
        {
            return _keyExtractor(obj).GetHashCode();
        }
    }

    public class StrictKeyEqualityComparer<T, TKey> : KeyEqualityComparer<T, TKey>
        where TKey : IEquatable<TKey>
    {
        public StrictKeyEqualityComparer(Func<T, TKey> keyExtractor)
            : base(keyExtractor)
        { }

        public override bool Equals(T x, T y)
        {
            // This will use the overload that accepts a TKey parameter
            // instead of an object parameter.
            return _keyExtractor(x).Equals(_keyExtractor(y));
        }
    }
}
