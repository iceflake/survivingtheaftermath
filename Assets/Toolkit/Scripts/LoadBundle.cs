using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public static class LoadBundle
{
    static string modPath = System.Environment.ExpandEnvironmentVariables("%USERPROFILE%\\Documents\\Paradox Interactive\\Surviving The Aftermath\\Mods\\");

    const byte modVersion = 0;
    static readonly byte[] versionTag = new byte[] { (byte)'I', (byte)'C', (byte)'E', (byte)'M', modVersion };

    [MenuItem("Mod/Build Mod")]
    static void BuildABs()
    {
        string buildPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + "Mods";

        if (!Directory.Exists(buildPath))
            Directory.CreateDirectory(buildPath);

        AssetBundleBuild[] buildMap = new AssetBundleBuild[2];

        buildMap[1].assetBundleName = "ingame";
        buildMap[1].assetNames = new string[]
        {
            "Assets/Configurations",
        };

        string[] modGuids = AssetDatabase.FindAssets("t:Mod");

        foreach(var guid in modGuids)
        {
            string modAssetPath = AssetDatabase.GUIDToAssetPath(guid);
            string directory = Path.GetDirectoryName(modAssetPath);
            string name = Path.GetFileName(Path.GetDirectoryName(modAssetPath));

            buildMap[0].assetBundleName = name.ToLower().Replace(' ', '_');
            buildMap[0].assetBundleVariant = "";
            buildMap[0].assetNames = new string[] { directory };
        }

        BuildPipeline.BuildAssetBundles("Mods", buildMap, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);

        if (!Directory.Exists(modPath))
            Directory.CreateDirectory(modPath);

        string[] files = Directory.GetFiles("Mods", "*.mod");

        foreach (string file in files)
        {
            using (var stream = new FileStream(file, FileMode.Append))
            {
                stream.Write(versionTag, 0, versionTag.Length);
                stream.Close();
            }

            FileInfo file_info = new FileInfo(file);
            File.Copy(file, modPath + file_info.Name, true);
        }

        if(EditorUtility.DisplayDialog("Mod build complete", "Select Launch to launch the mod ingame.\n\nBuilt mods are located in: " + modPath, "Launch", "Continue"))
        {
            System.Diagnostics.Process.Start("com.epicgames.launcher://apps/Muscovy?action=launch&silent=true");
        }
    }
}
#endif