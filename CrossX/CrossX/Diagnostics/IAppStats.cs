namespace CrossX.Diagnostics
{
    public interface IAppStats
    {
        int TotalDrawCallsLastFrame { get; }
        int CurrentDrawCallsInFrame { get; }
        int TotalVerticesLastFrame { get; }
        int CurrentVerticesInFrame { get; }
        float Fps { get; }
    }
}
