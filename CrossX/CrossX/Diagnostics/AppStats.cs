namespace CrossX.Diagnostics
{
    internal class AppStats: IAppStats
    {
        public int TotalDrawCallsLastFrame { get; set; }
        public int CurrentDrawCallsInFrame { get; set; }
        public int TotalVerticesLastFrame { get; set; }
        public int CurrentVerticesInFrame { get; set; }
        public float Fps { get; set; }
    }
}
