using System.Collections.Generic;
using S8l.CustomPlatformSharedInterfaces.Runtime;

namespace S8l.CustomPlatformDetector.Runtime
{
    public static class CustomPlatformUtils
    {
        public static List<IPlatform> GetIntersection(List<IPlatform> x, List<IPlatform> y)
        {
            List<IPlatform> result = new List<IPlatform>();
            
            foreach (var platformX in x)
            {
                foreach (var platformY in y)
                {
                    if (platformX.Equals(platformY))
                    {
                        result.Add(platformX);
                    }
                }
            }

            return result;
        }




        public static bool HasIntersection(List<IPlatform> x, List<IPlatform> y)
        {
                        
            foreach (var platformX in x)
            {
                foreach (var platformY in y)
                {
                    if (platformX.Equals(platformY))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public static bool HasIntersection(List<IBuildPlatform> x, List<IBuildPlatform> y)
        {
                        
            foreach (var platformX in x)
            {
                foreach (var platformY in y)
                {
                    if (platformX.Equals(platformY))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        
        
        
    }
}