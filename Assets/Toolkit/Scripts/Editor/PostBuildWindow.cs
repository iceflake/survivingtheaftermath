using System.IO;
using UnityEditor;
using UnityEngine;

public class PostBuildWindow : EditorWindow
{
    GUIStyle richTextStyle;
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
        richTextStyle = new GUIStyle(EditorStyles.boldLabel);
        richTextStyle.richText = true;
        richTextStyle.alignment = TextAnchor.MiddleCenter;
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("<size=20>Mod build complete!</size>", richTextStyle);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Select Launch to launch the mod ingame.", EditorStyles.wordWrappedLabel);
        EditorGUILayout.LabelField($"Built mods are located in:\n{modPath}", EditorStyles.wordWrappedLabel);
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
