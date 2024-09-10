using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace S8l.OdinInspector.Editor
{
    public static class OdinFileUpdater
    {
        [MenuItem("Tools/Odin Inspector/Custom/Update dll")]
        public static void UpdateDll()
        {
            var dirs = Directory.GetDirectories("Library/PackageCache", "*odin*");
            AOTGenerationConfig.Instance.ScanProject();
            AOTGenerationConfig.Instance.GenerateDLL();
            File.Delete(dirs[0] + "/Plugins/Sirenix/Assemblies/AOT/link.xml");
            if (File.Exists("Assets/Odin/Sirenix.Serialization.AOTGenerated.dll"))
            {
                File.Delete("Assets/Odin/Sirenix.Serialization.AOTGenerated.dll");
            }
            File.Move(dirs[0] + "/Plugins/Sirenix/Assemblies/AOT/Sirenix.Serialization.AOTGenerated.dll", "Assets/Odin/Sirenix.Serialization.AOTGenerated.dll");
            Directory.Delete(dirs[0] + "/Plugins/Sirenix/Assemblies/AOT");
            Debug.Log("Generated new AOTGenerated.dll file");
        }
    }
}