using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microex.All.Common
{
    public abstract class Enumeration<T> : ValueObject,IComparable where T : IComparable
    {
        public string Name { get; }
        public T Value { get; }

        protected Enumeration(T value, string name)
        {
            Value = value;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator T(Enumeration<T> value)
        {
            return value.Value;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
            yield return Value;
        }

        public static bool TryParse<TImplemetions>(T value,out TImplemetions result) where TImplemetions:Enumeration<T>
        {
            result = Enumeration<T>.GetAll<TImplemetions>().FirstOrDefault(x => x.Value.CompareTo(value) == 0);
            return result != default(TImplemetions);
        }

        public static IEnumerable<TImplemention> GetAll<TImplemention>() where TImplemention : Enumeration<T>
        {
            var type = typeof(TImplemention);
            var typeInfo = type.GetTypeInfo();
            var fields = typeInfo.GetFields(BindingFlags.Public |
                                                      BindingFlags.Static |
                                                      BindingFlags.DeclaredOnly);
            foreach (var info in fields)
            {
                if (info.GetValue(default(TImplemention)) is TImplemention locatedValue)
                {
                    yield return locatedValue;
                }
            }
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration<T>;
            if (otherValue == null)
            {
                return false;
            }
            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Value.Equals(otherValue.Value);
            return typeMatches && valueMatches;
        }

        public int CompareTo(object other)
        {
            return Value.CompareTo(((Enumeration<T>)other).Value);
        }

        // Other utility methods ...
    }
}
