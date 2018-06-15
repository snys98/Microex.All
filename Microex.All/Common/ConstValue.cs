using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microex.All.Extensions;

namespace Microex.All.Common
{
    public abstract class ConstValue<T> : IComparable
    {
        public string Name { get; }
        public T Value { get; }

        protected ConstValue()
        {
            
        }
        protected ConstValue(string name,T value)
        {
            Value = value;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator T(ConstValue<T> value)
        {
            return value.Value;
        }

        public static TImplemetions Parse<TImplemetions>(T value) where TImplemetions : ConstValue<T>, new()
        {
            return ConstValue<T>.GetAll<TImplemetions>().First();
        }

        public static IEnumerable<TImplemention> GetAll<TImplemention>() where TImplemention : ConstValue<T>, new()
        {
            var type = typeof(TImplemention);
            var typeInfo = type.GetTypeInfo();
            var fields = typeInfo.GetFields(BindingFlags.Public |
                                                      BindingFlags.Static |
                                                      BindingFlags.DeclaredOnly);
            foreach (var info in fields)
            {
                var instance = new TImplemention();
                if (info.GetValue(instance) is TImplemention locatedValue)
                {
                    yield return locatedValue;
                }
            }
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as ConstValue<T>;
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
            return String.Compare(Value.ToJson(), ((ConstValue<T>)other).Value.ToJson(), StringComparison.Ordinal);
        }

        // Other utility methods ...
    }
}
