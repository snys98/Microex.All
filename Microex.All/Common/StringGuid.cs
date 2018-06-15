using System;

namespace Microex.All.Common
{
    public class StringGuid: IEquatable<StringGuid>
    {
        private Guid _guid;
        public bool Equals(StringGuid other)
        {
            if (other == null)
            {
                return false;
            }
            return this._guid.ToString("N") == other.ToString();
        }

        public StringGuid(Guid guid)
        {
            this._guid = guid;
        }

        public StringGuid(string guid):this(Guid.Parse(guid))
        {

        }

        public StringGuid():this(Guid.NewGuid())
        {
            
        }

        public static StringGuid New()
        {
            return new StringGuid();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StringGuid) obj);
        }

        public static implicit operator string(StringGuid stringGuid)
        {
            return stringGuid.ToString();
        }

        public static implicit operator StringGuid(string guid)
        {
            return new StringGuid(guid);
        }

        public override string ToString()
        {
            return this._guid.ToString("N");
        }
    }
}
