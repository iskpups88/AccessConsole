using System;
using AccessConsole.Enums;

namespace AccessConsole
{
    public class Key : IEquatable<Key>
    {
        public string User { get; }
        public Objects Object { get; }

        public Key(string user, Objects o)
        {
            User = user;
            Object = o;
        }


        public bool Equals(Key other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return User == other.User && Object == other.Object;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Key) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(User, (int) Object);
        }
    }
}