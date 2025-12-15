using System;
using System.Collections.Generic;

namespace TapMatch.Tests.EditMode
{
    public class MockRandom : Random
    {
        private readonly Queue<int> Sequence;

        public MockRandom(IEnumerable<int> fixedSequence)
        {
            Sequence = new Queue<int>(fixedSequence);
        }

        public override int Next(int maxValue)
        {
            if (Sequence.Count == 0)
                throw new InvalidOperationException("MockRandom sequence is empty.");

            return Sequence.Dequeue() % maxValue;
        }

        public override int Next()
        {
            return Sequence.Count == 0
                ? throw new InvalidOperationException("MockRandom sequence is empty.")
                : Sequence.Dequeue();
        }
    }
}