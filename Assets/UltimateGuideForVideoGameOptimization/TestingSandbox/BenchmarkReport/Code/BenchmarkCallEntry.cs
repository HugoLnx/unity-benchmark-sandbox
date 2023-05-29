namespace BenchmarkReport
{
    public struct BenchmarkCallEntry
    {
        public string Title { get; set; }
        public int ReferenceId { get; set; }
        public bool IsReference => ReferenceId > 0;
        public float Percentage { get; set; }
        public float[] ReferenceComparisons { get; set; }
        public float Avg { get; set; }
        public float Max { get; set; }
        public float Min { get; set; }
    }
}
