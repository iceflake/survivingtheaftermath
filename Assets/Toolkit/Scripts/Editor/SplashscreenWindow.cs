using UnityEditor;
using UnityEngine;

public class SplashscreenWindow : EditorWindow
{
    const int WINDOW_WIDTH = 550;
    const int WINDOW_HEIGHT = 440;

    GUIStyle richText;

    bool noSplashScreen = true;

    [MenuItem("Help/About Modtools", false, 0)]
    public static void ExecuteMenuItem()
    {
        SplashscreenWindow.ShowWindow();
    }

    public static void ShowWindow()
    {
        EditorWindow editorWindow = GetWindow<SplashscreenWindow>(true, "Welcome", true);

        editorWindow.position = new Rect((Screen.width) / 2f, 175, WINDOW_WIDTH, WINDOW_HEIGHT);

        editorWindow.maxSize = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
        editorWindow.minSize = new Vector2(WINDOW_WIDTH, 200);

        editorWindow.ShowUtility();
    }

    private void Awake()
    {
        richText = new GUIStyle();
        richText.richText = true;

        noSplashScreen = EditorPrefs.GetBool("Aftermath_NoSplashscreen", true);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("<size=20><color=white>Welcome to Surviving the Aftermath Mod tool</color></size>", richText);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUI.enabled = false;
        GUILayout.TextArea("<color=white>You can use this tool for modding. More information can be found in the wiki at:\nhttps://sta.paradoxwikis.com/\n\nMore information coming soon.</color>", richText, GUILayout.Height(position.height - 70));
        GUI.enabled = true;

        EditorGUILayout.BeginHorizontal();
        noSplashScreen = EditorGUILayout.ToggleLeft("Don't show this again", noSplashScreen);
        if(GUILayout.Button("Continue"))
        { 
            Close();
        }
        EditorGUILayout.EndHorizontal(); 

        EditorGUILayout.EndVertical();


    }

    private void OnDestroy() 
    {
        EditorPrefs.SetBool("Aftermath_NoSplashscreen", noSplashScreen);
    }
}
