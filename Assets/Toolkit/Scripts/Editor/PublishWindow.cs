using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using UnityEditor;
using UnityEngine;

public class PublishWindow : EditorWindow
{
    GUIStyle richTextStyle;
    
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
        richTextStyle = new GUIStyle(EditorStyles.boldLabel);
        richTextStyle.richText = true;
    }

    void OnEnable()
    {
        TryToFindApplicationPath();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("<size=20>Publish your mod</size>", richTextStyle);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        DisplayMod();
        DisplayActions();
    }

    void DisplayMod()
    {
        if (!string.IsNullOrEmpty(modPath))
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mod: ", GUILayout.Width(60));
            EditorGUILayout.LabelField(Path.GetFileName(modPath), EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Path: ", GUILayout.Width(60));
        EditorGUILayout.LabelField(!string.IsNullOrEmpty(modPath) ? modPath : "No path selected", EditorStyles.wordWrappedLabel);
        EditorGUILayout.EndHorizontal();

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

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("<size=16>Steam Workshop</size>", richTextStyle);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("You need to own Surviving the Aftermath in Steam to be able to upload the mod to Steam Workshop.", MessageType.Info);
        EditorGUILayout.LabelField("Surviving the Aftermath installation path:", EditorStyles.wordWrappedLabel); 
        EditorGUILayout.LabelField(string.IsNullOrEmpty(applicationPath) ? string.Empty : applicationPath, EditorStyles.wordWrappedLabel);

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

        EditorGUILayout.EndVertical();
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
