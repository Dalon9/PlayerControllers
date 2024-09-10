using UnityEngine;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Object = UnityEngine.Object;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;

#endif

namespace S8l.BaseConfiguration
{
#if ODIN_INSPECTOR
    public class PackageConfiguration : SerializedScriptableObject
#else
    public class PackageConfiguration : ScriptableObject
#endif
    {
        protected const string MenuName = "Configurations/";
        protected const string ConfigPath = "ManagerConfigs/";
        protected const string DefaultConfigPath = "DefaultConfigs/";

        public static T GetCurrentConfiguration<T>(out PackageConfigurationLoadingResult result) where T : Object
        {
            return (T) GetCurrentConfiguration(typeof(T), out result);
        }

        public static Object GetCurrentConfiguration(Type type, out PackageConfigurationLoadingResult result)
        {
            // Load project config first
            var resources = Resources.LoadAll(ConfigPath, type);
            if (resources.Length >= 1)
            {
                result = resources.Length > 1
                    ? PackageConfigurationLoadingResult.MultipleProject
                    : PackageConfigurationLoadingResult.ProjectConfig;
                return resources[0];
            }

            // If nothing found, load default config
            resources = Resources.LoadAll(DefaultConfigPath, type);
            if (resources.Length >= 1)
            {
                result = resources.Length > 1
                    ? PackageConfigurationLoadingResult.MultipleDefault
                    : PackageConfigurationLoadingResult.DefaultConfig;
                return resources[0];
            }

            result = PackageConfigurationLoadingResult.NoConfig;
            return null;
        }

        public static Type[] GetAllConfigurationTypes()
        {
            var resources = Resources.LoadAll(string.Empty, typeof(PackageConfiguration));
            var types = resources.Select(x => x.GetType()).Distinct();

            return types.ToArray();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                AssetDatabase.SaveAssets();
            }
                
        }
#endif
    }

    public enum PackageConfigurationLoadingResult
    {
        NoConfig,
        DefaultConfig,
        ProjectConfig,
        MultipleDefault,
        MultipleProject
    }
}