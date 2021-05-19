using Aftermath.Mods;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(Mod))]
public class ModEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        HandleGenericProperty("icon");
        HandleGenericProperty("modname");
        HandleGenericProperty("desc");
        HandleGenericProperty("version");
        HandleGenericProperty("startID");
        HandleTags();
    }

    void HandleGenericProperty(string propertyName)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(property, true);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    void HandleTags()
    {
        SerializedProperty tagsProperty = serializedObject.FindProperty("tags");

        GUILayout.BeginVertical();

        if (GUILayout.Button("Select tags"))
        {
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < Mod.AvailableTags.Count; i++)
            {
                AddMenuItem(menu, Mod.AvailableTags[i], StringListPropertyContainsString(tagsProperty, Mod.AvailableTags[i]));
            }

            menu.ShowAsContext();
        }
        
        EditorGUILayout.LabelField("Current tags:", EditorStyles.boldLabel);

        if (tagsProperty.arraySize > 0)
        {
            for (int i = 0; i < tagsProperty.arraySize; i++)
            {
                EditorGUILayout.LabelField("- " + tagsProperty.GetArrayElementAtIndex(i).stringValue);
            }
        }
        else
        {
            EditorGUILayout.LabelField("- None");
        }

        GUILayout.EndVertical();
    }

    bool StringListPropertyContainsString(SerializedProperty property, string str)
    {
        return StringListPropertyContainsString(property, str, out int index);
    }

    bool StringListPropertyContainsString(SerializedProperty property, string str, out int index)
    {
        index = -1;

        if (string.IsNullOrEmpty(str))
        {
            return false;
        }

        int count = property.arraySize;
        for (int i = 0; i < count; i++)
        {
            var propertyString = property.GetArrayElementAtIndex(i)?.stringValue;

            if (string.IsNullOrEmpty(propertyString))
            {
                continue;
            }

            if (propertyString.Equals(str))
            {
                index = i;

                return true;
            }
        }

        return false;
    }

    void AddMenuItem(GenericMenu menu, string tag, bool isActive)
    {
        GenericMenu.MenuFunction2 func = null;

        if (isActive)
        {
            func = RemoveTag;
        }
        else
        {
            func = AddTag;
        }

        menu.AddItem(new GUIContent(tag), isActive, func, tag);
    }

    void AddTag(object tag)
    {
        string tagString = tag as string;

        if (!string.IsNullOrEmpty(tagString))
        {
            SerializedProperty tagsProperty = serializedObject.FindProperty("tags");

            int index = tagsProperty.arraySize;

            tagsProperty.InsertArrayElementAtIndex(index);
            tagsProperty.GetArrayElementAtIndex(index).stringValue = tagString;

            serializedObject.ApplyModifiedProperties();
        }
    }

    void RemoveTag(object tag)
    {
        string tagString = tag as string;

        if (!string.IsNullOrEmpty(tagString))
        {
            SerializedProperty tagsProperty = serializedObject.FindProperty("tags");

            if (StringListPropertyContainsString(tagsProperty, tagString, out int index))
            {
                tagsProperty.DeleteArrayElementAtIndex(index);

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif
