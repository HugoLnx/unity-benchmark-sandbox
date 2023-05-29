namespace BenchmarkReport
{
    public struct BenchmarkTotalEntry
    {
        public float Avg { get; set; }
        public float Max { get; set; }
        public float Min { get; set; }
        public float Percent80FPS { get; set; }
        public float Percent60FPS { get; set; }
    }
}
