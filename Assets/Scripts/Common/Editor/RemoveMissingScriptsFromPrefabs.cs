using UnityEditor;
using UnityEngine;

namespace Common.Editor
{
    public static class RemoveMissingScriptsFromPrefabs
    {
        [MenuItem("Tools/Cleanup/Remove Missing Scripts From Prefabs")]
        public static void RemoveMissingScripts()
        {
            // Znajdź wszystkie prefaby w projekcie
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

            int totalRemoved = 0;
            int affectedPrefabs = 0;

            try
            {
                AssetDatabase.StartAssetEditing();

                for (int i = 0; i < prefabGuids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                    if (prefab == null)
                        continue;

                    int removedOnThisPrefab = 0;

                    // Przejdź po wszystkich obiektach w prefabie (włącznie z dziećmi, ukrytymi itp.)
                    var allTransforms = prefab.GetComponentsInChildren<Transform>(true);
                    foreach (var t in allTransforms)
                    {
                        removedOnThisPrefab += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
                    }

                    if (removedOnThisPrefab > 0)
                    {
                        totalRemoved += removedOnThisPrefab;
                        affectedPrefabs++;

                        Debug.Log($"[{path}] Removed {removedOnThisPrefab} missing script(s).", prefab);
                        EditorUtility.SetDirty(prefab);
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
            }

            Debug.Log($"Finished! Removed {totalRemoved} missing script(s) from {affectedPrefabs} prefab(s).");
        }
    }
}