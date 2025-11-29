namespace Shared.Enums
{
    public abstract class ValueObject
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; } = string.Empty;
        public override string ToString()
        {
            return Name;
        }
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, right) || left!.Equals(right!);
        }

        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !EqualOperator(left, right);
        }

        protected virtual IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
            yield return Name;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }
        // Other utility methods
    }
}
