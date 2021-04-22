using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using UnityEditor;
using UnityEngine;

public class PublishWindow : EditorWindow
{
    GUIStyle richTextStyle;
    GUIStyle basicTextStyle;
    
    string applicationPath;
    string modPath;

    [MenuItem("Mod/Publish Mod on Steam Workshop")]
    public static void Publish()
    {
        Open(string.Empty);
    }

    public static void Open(string modPath)
    {
        var window = GetWindow<PublishWindow>(true, "Publish mod", true);
        window.modPath = modPath;
        window.maxSize = new Vector2(800, 500);
        window.minSize = new Vector2(400, 500);
        window.TryToFindApplicationPath();

        window.ShowUtility();
    }

    void Awake()
    {
        richTextStyle = new GUIStyle();
        richTextStyle.richText = true;

        basicTextStyle = new GUIStyle();
        basicTextStyle.normal.textColor = Color.white;
        basicTextStyle.wordWrap = true;
        basicTextStyle.padding = new RectOffset(4, 4, 4, 4);
    }

    void OnEnable()
    {
        TryToFindApplicationPath();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("<size=20><color=white><b>Publish your mod</b></color></size>", richTextStyle);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        DisplayMod();
        DisplayActions();
    }

    void DisplayMod()
    {
        if (!string.IsNullOrEmpty(modPath))
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Mod: ", basicTextStyle, GUILayout.Width(100));
            GUILayout.Label(Path.GetFileName(modPath), basicTextStyle);
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Path:", basicTextStyle, GUILayout.Width(100));
        GUILayout.Label(!string.IsNullOrEmpty(modPath) ? modPath : "No path selected", basicTextStyle);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Select mod path"))
        {
            string path = EditorUtility.OpenFilePanel("Mod to publish", "", "mod");
            if (path.Length != 0)
            {
                modPath = path;
            }
        }
    }

    void DisplayActions()
    {
        if (string.IsNullOrEmpty(modPath) || !File.Exists(modPath))
        {
            return;
        }

        DisplaySteamWorkshopActions();
    }

    void DisplaySteamWorkshopActions()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("<size=16><color=white><b>Steam Workshop</b></color></size>", richTextStyle);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("You need to own Surviving the Aftermath in Steam to be able to upload the mod to Steam Workshop.", MessageType.Info);
        GUILayout.Label("Surviving the Aftermath installation path:", basicTextStyle); 
        GUILayout.Label(string.IsNullOrEmpty(applicationPath) ? string.Empty : applicationPath, basicTextStyle);

        if (string.IsNullOrEmpty(applicationPath))
        {
            EditorGUILayout.HelpBox("Path to Aftermath64.exe not set", MessageType.Error);
        }
        else
        {
            if (GUILayout.Button("Publish on Steam Workshop"))
            {
                Process process = new Process();
                process.StartInfo.FileName = applicationPath;
                process.StartInfo.Arguments = $"--modtools steam {modPath}";

                process.Start();

                Close();
            }
        }

        GUILayout.EndVertical();
    }

    void TryToFindApplicationPath()
    {
        var path = GetInstallationPath();

        if (string.IsNullOrEmpty(path))
        {
            UnityEngine.Debug.LogError(
                "Unable to find Surviving the Aftermath installation in registry");

            return;
        }

        applicationPath = path;
    }

    static string GetInstallationPath()
    {
        string exePath = string.Empty;

        if (Environment.Is64BitOperatingSystem)
        {
            exePath = GetValueFromSubkey(Registry.LocalMachine,
                "SOFTWARE\\Wow6432Node\\Iceflake Studios\\Surviving the Aftermath",
                "Exe_Path");
        }

        return exePath;
    }

    static string GetValueFromSubkey(RegistryKey registryKey, string subKey, string key)
    {
        using (RegistryKey registrySubKey = registryKey.OpenSubKey(subKey))
        {
            return registrySubKey?.GetValue(key)?.ToString() ?? string.Empty;
        }
    }
}
