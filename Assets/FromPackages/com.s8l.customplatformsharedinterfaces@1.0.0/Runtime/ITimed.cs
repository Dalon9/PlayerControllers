namespace S8l.CustomPlatformSharedInterfaces.Runtime
{
    public interface ITimed
    {

        Timing Timer { get; }

        float Unit { get; }

    }
    
    
    public enum Timing
    {
        Start,
        Init,
        Frame,
        Seconds
    }
}