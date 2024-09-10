using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;

namespace S8l.OdinInspector.Editor
{
    public class OdinFileGenerator : ScriptableObject
    {
#if UNITY_EDITOR
        private bool projectOpened = false;

        private void OnEnable()
        {
            if (projectOpened)
                return;

            projectOpened = true;

            if (PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup) != ApiCompatibilityLevel.NET_4_6)
                Debug.LogWarning("Please set the API compatibility level to 4.x for Odin to work!");

            var dirs = Directory.GetDirectories("Library/PackageCache", "*odin*");

            if (AssetDatabase.FindAssets("Sirenix.Serialization.AOTGenerated").Length == 0)
            {
                if (!Directory.Exists("Assets/Odin"))
                {
                    Directory.CreateDirectory("Assets/Odin");
                }
                if (dirs.Length > 0)
                {
                    File.Copy(dirs[0] + "/Samples~/Link/link.xml", "Assets/Odin/link.xml");
                    File.Copy(dirs[0] + "/Samples~/Link/AOTGenerationConfig.asset", "Assets/Odin/AOTGenerationConfig.asset");
                    File.Copy(dirs[0] + "/Samples~/Link/ImportSettingsConfig.asset", "Assets/Odin/ImportSettingsConfig.asset");
                    File.Copy(dirs[0] + "/Samples~/Link/GlobalSerializationConfig.asset", "Assets/Odin/GlobalSerializationConfig.asset");
                    File.Copy(dirs[0] + "/Samples~/Link/GeneralDrawerConfig.asset", "Assets/Odin/GeneralDrawerConfig.asset");
                    Debug.Log("Copied link.xml and relevant config files to Assets/Odin folder.");
                    EditorCoroutineUtility.StartCoroutineOwnerless(HandleFiles());
                }
            }
        }

        private IEnumerator HandleFiles()
        {
            AssetDatabase.Refresh();
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Handling");
            var dirs = Directory.GetDirectories("Library/PackageCache", "*odin*");
            AOTGenerationConfig.Instance.ScanProject();
            AOTGenerationConfig.Instance.GenerateDLL();
            ImportSettingsConfig.Instance.AutomateBeforeBuild = false;
            File.Delete(dirs[0] + "/Plugins/Sirenix/Assemblies/AOT/link.xml");
            File.Move(dirs[0] + "/Plugins/Sirenix/Assemblies/AOT/Sirenix.Serialization.AOTGenerated.dll", "Assets/Odin/Sirenix.Serialization.AOTGenerated.dll");
            Directory.Delete(dirs[0] + "/Plugins/Sirenix/Assemblies/AOT");
            Debug.Log("Set AutomateBeforeBuild in ImportSettingsConfig to false");
            Debug.Log("Created Sirenix.Serialization.AOTGenerated.dll in Assets/Odin folder");
            AssetDatabase.Refresh();
        }
#endif
    }
}