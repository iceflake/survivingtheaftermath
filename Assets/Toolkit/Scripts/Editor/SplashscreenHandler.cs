using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SplashscreenHandler : MonoBehaviour
{
    static SplashscreenHandler()
    {
        if (!EditorPrefs.GetBool("Aftermath_NoSplashscreen")) 
        {
            SplashscreenWindow.ShowWindow();  
        }
    }
}
