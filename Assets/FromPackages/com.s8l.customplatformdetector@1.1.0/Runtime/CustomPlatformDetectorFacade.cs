using System.Collections.Generic;
using System.Linq;
using S8l.BaseConfiguration;
using S8l.CustomPlatformDetector.Runtime.Platforms;
using S8l.CustomPlatformSharedInterfaces.Runtime;
using Sirenix.Utilities;
using UnityEngine;

namespace S8l.CustomPlatformDetector.Runtime
{
    /// <summary>
    /// Central facade class to resolve which platform we are on
    /// </summary>
    public static class CustomPlatformDetectorFacade
    {
        
        private static CustomPlatformDetectorConfig config = null;

        private static List<IPlatform> cachedResult = null;

        /// <summary>
        /// Central facade method to retrieve the current platforms.
        /// </summary>
        /// <returns></returns>
        public static List<IPlatform> DetectPlatforms(bool orderBy = false)
        {



            if(TryToResolveConfig() == false)
                return new List<IPlatform>(){new Any()};
            


            List<IPlatform> results;
            
            // Override from config?
            if (config.DoOverrideWith != null && config.DoOverrideWith.Count > 0)
            {
                results = config.DoOverrideWith;
            }
            else // Find matching platforms
            {
                if (cachedResult.IsNullOrEmpty())
                {
                    cachedResult = FilterMatches(); // Do the hard work
                }

                results =  cachedResult;
            }

            // Sort by Generality if wanted, more specific are first
            if (orderBy == true)
            {
                results = results.OrderBy(o => o.Generality).ToList();
            }

            return results;



        }


        /// <summary>
        /// Resolve the matching platforms.
        /// </summary>
        /// <returns>A list of matching platforms</returns>
        private static List<IPlatform> FilterMatches()
        {
            
            List<IPlatform> matches = new List<IPlatform>();

            foreach (IPlatform p in config.KnownPlatforms)
            {
                if (p.IsThisMyPlatform() == true)
                {
                    matches.Add(p);
                }
            }

            return matches;


        }


        /// <summary>
        /// Load the matches for the current build target, this is not affected by the override list and not sorted in any particluar order
        /// </summary>
        /// <returns>List of detected build targets</returns>
        public static List<IBuildPlatform> GetBuildPlatforms()
        {
#if UNITY_EDITOR

            List<IBuildPlatform> result = new List<IBuildPlatform>();
            
            if(TryToResolveConfig() == false)
                return new List<IBuildPlatform>(){new Any()};

            foreach (var platform in config.KnownPlatforms)
            {
                if (platform is IBuildPlatform && (platform as IBuildPlatform).IsThisBuildPlatform() == true)
                {
                    result.Add((IBuildPlatform)platform);
                }
            }

            return result;
#else

            return new List<IBuildPlatform>(); // Return empty list outside of editor 
            
#endif
        }


        /// <summary>
        /// Try to resolve config if not already present
        /// </summary>
        /// <returns>Success</returns>
        public static bool TryToResolveConfig()
        {
            
            if (config == null)
            {
                PackageConfigurationLoadingResult res;
                config = PackageConfiguration.GetCurrentConfiguration<CustomPlatformDetectorConfig>(out res);

                // Failed retrieving config
                if (config == null)
                {
                    Debug.LogError("No package configuration found for PlatformDetector, could be any platform!");
                    return false;
                }
            }

            return true;
        }
        
        


    }
}

