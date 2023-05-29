using UnityEngine;

namespace BenchmarkReport
{
    public class Measurer
    {
        private float NowInMillis => (float) (Time.realtimeSinceStartupAsDouble * 1000.0);
        public float UpToNow => NowInMillis - _startedAt;
        private float _startedAt;

        public void Restart()
        {
            _startedAt = NowInMillis;
        }
    }
}
