Make sure API compatibility level is set to .NET 4.x in Player Settings.

All imports and settings should be done automatically when importing the package.
In case you have to do it manually for some reason:

1. Import Samples - Link, AOTGenConfig, Link, AOTGenConfig, ImportConfig from the package manager window.
     You will see three files: link.xml, AOTGenerationConfig.asset and ImportConfig.asset.
     You can move both files into a folder that's more suitable, e.g. Assets/Plugins/

3. Before building the project, select AOTGenerationConfig.asset from the previous step
     and Scan Project, then Generate DLL.

4. It will produce errors in the console as the package is read-only. 
     However, open the package folder in File Explorer
     ( \Library\PackageCache\com.s8l.plugins.odin@x\Plugins\Sirenix\Assemblies\AOT\ )
     and you will see that Sirenix.Serialization.AOTGenerated.dll and link.xml have been generated.
     Delete link.xml file.  Move the dll file into Assets folder, e.g. Assets/Plugins

5. In case no custom serialized types were used and Sirenix.Serialization.AOTGenerated.dll is not
     generated, delete its linker line from link.xml in Assets. Add it back if AOT dll is generated.

6. To be able to use Build And Run, select the ImportConfig.asset and uncheck 

7. When switching platform, repeating steps 3-5 might be necessary, 
     i.e. generate new dll and replace the old one.