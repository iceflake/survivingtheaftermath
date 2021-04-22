using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PostBuildWindow : EditorWindow
{
    GUIStyle richTextStyle;
    GUIStyle basicTextStyle;
    string modPath;
    string[] filePaths;

    public static void Open(string modPath, string[] filePaths)
    {
        var window = GetWindow<PostBuildWindow>(true, "Mod build complete", true);
        window.maxSize = new Vector2(500, 500);
        window.minSize = new Vector2(500, 500);
        window.modPath = modPath;
        window.filePaths = filePaths;

        window.ShowUtility();
    }

    void Awake()
    {
        richTextStyle = new GUIStyle();
        richTextStyle.richText = true;
        richTextStyle.alignment = TextAnchor.MiddleCenter;

        basicTextStyle = new GUIStyle();
        basicTextStyle.normal.textColor = Color.white;
        basicTextStyle.wordWrap = true;
        basicTextStyle.padding = new RectOffset(4, 4, 4, 4);
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("<size=20><color=white><b>Mod build complete!</b></color></size>", richTextStyle);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Select Launch to launch the mod ingame.", basicTextStyle);
        EditorGUILayout.LabelField($"Built mods are located in:\n{modPath}", basicTextStyle);
        EditorGUILayout.Space();

        GUILayout.BeginVertical();
        if (GUILayout.Button("Launch Epic"))
        {
            System.Diagnostics.Process.Start("com.epicgames.launcher://apps/Muscovy?action=launch&silent=true");
            Close();
        }
        if (GUILayout.Button("Launch Steam"))
        {
            System.Diagnostics.Process.Start("steam://rungameid/684450");
            Close();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Publish on Steam Workshop"))
        {
            PublishWindow.Open(Path.Combine(modPath, Path.GetFileName(filePaths[0])));
            Close();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Continue"))
        {
            Close();
        }
        GUILayout.EndVertical();
    }
}
