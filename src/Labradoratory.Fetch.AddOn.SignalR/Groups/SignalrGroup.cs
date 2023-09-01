using System.Linq;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups
{
    public class SignalrGroup
    {
        public static char GroupSeparator = '.';
        private readonly object[] _parts;

        public static SignalrGroup Create(params object[] parts)
        {
            return new SignalrGroup(parts);
        }

        private SignalrGroup(params object[] parts)
        {
            _parts = parts;
        }

        public SignalrGroup Prepend(params object[] parts)
        {
            return Create(parts.Concat(_parts).ToArray());
        }

        public SignalrGroup Append(params object[] parts)
        {
            return Create(_parts.Concat(parts).ToArray());
        }

        public override string ToString()
        {
            return string.Join(GroupSeparator, _parts).ToLower();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is SignalrGroup g)
                return ToString() == g.ToString();

            return false;
        }

        public static implicit operator string(SignalrGroup group)
        {
            return group.ToString();
        }
    }
}
