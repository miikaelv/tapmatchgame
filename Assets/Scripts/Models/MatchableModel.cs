using System;
using TapMatch.Models.Configs;

namespace TapMatch.Models
{
    [Serializable]
    public readonly struct MatchableModel : IEquatable<MatchableModel>
    {
        public Guid Id { get; }
        public MatchableType Type { get; }
        public bool IsEmpty => Type == MatchableType.None;
        
        public static MatchableModel Empty => new ();

        public MatchableModel(MatchableType type)
        {
            Id = Guid.NewGuid();
            Type = type;
        }

        public bool Equals(MatchableModel other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is MatchableModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
        
        public static bool operator == (MatchableModel m1, MatchableModel m2) => m1.Equals(m2);
        public static bool operator != (MatchableModel m1, MatchableModel m2) => !m1.Equals(m2);
    }
}