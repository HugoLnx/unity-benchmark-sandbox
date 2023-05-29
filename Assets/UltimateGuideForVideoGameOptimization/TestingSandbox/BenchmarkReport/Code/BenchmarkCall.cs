using System;
using System.Reflection;

namespace BenchmarkReport
{
    public class BenchmarkCall
    {
        public string Name { get; }
        public Action Action { get; }
        public MovingAverage MovingAverage { get; }
        public int ReferenceId { get; }
        public int? Iterations { get; }
        public float TimeMultiplier { get; }

        public bool IsReference => ReferenceId > 0;

        public BenchmarkCall(string name, System.Action action, MovingAverage movingAverage, int referenceId, int? iterations = null, float? timeMultiplier = 1f)
        {
            Name = name;
            Action = action;
            MovingAverage = movingAverage;
            ReferenceId = referenceId;
            Iterations = iterations;
            TimeMultiplier = timeMultiplier.Value;
        }
    }
}
