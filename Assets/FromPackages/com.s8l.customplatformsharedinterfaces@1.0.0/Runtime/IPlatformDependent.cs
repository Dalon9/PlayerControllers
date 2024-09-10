using System.Collections.Generic;

namespace S8l.CustomPlatformSharedInterfaces.Runtime
{
    public interface IPlatformDependent
    {
        List<IPlatform> Platforms { get; }

        List<IPlatform> ButNotOn { get; }
    }
}