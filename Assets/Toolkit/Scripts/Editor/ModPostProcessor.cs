using System.Collections.Generic;
using UnityEditor;
using System;

namespace Aftermath
{
    public class ModPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            ProcessIDs<ProxyBuilding>((building) => building.ID = IdUtility.FindId<ProxyBuilding>());
            ProcessIDs<DisasterTemplate>((disaster) => disaster.ResetID());
        }

        private static void ProcessIDs<TIIdentifiable>(Action<TIIdentifiable> resetID)
            where TIIdentifiable : UnityEngine.ScriptableObject, IdUtility.IIdentifiable
        {
            List<TIIdentifiable> allObjects = new List<TIIdentifiable>();
            var guids = AssetDatabase.FindAssets($"t:{typeof(TIIdentifiable).Name}", null);

            foreach (string guid in guids)
            {
                allObjects.Add(AssetDatabase.LoadAssetAtPath<TIIdentifiable>(AssetDatabase.GUIDToAssetPath(guid)));
            }

            allObjects.Sort((a, b) => a.ID.CompareTo(b.ID));

            List<TIIdentifiable> problematics = new List<TIIdentifiable>();

            for (int i = 0; i < allObjects.Count; ++i)
            {
                var obj = allObjects[i];
                if (obj.ID < IdUtility.MinID)
                {
                    problematics.Add(obj);
                    continue;
                }

                if (i > 0)
                {
                    var previous = allObjects[i - 1];
                    if (obj.ID == previous.ID)
                    {
                        problematics.Add(obj);
                    }
                }
            }

            foreach (var obj in problematics)
            {
                resetID(obj);
                EditorUtility.SetDirty(obj);
            }
        }
    }
}