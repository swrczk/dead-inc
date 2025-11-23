#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NullReferenceFinder
{
    [MenuItem("Tools/swrczk/Find Null References In Scene")]
    public static void FindNullReferencesInScene()
    {
        var scene = SceneManager.GetActiveScene();
        var rootObjects = scene.GetRootGameObjects();

        int missingScriptsCount = 0;
        int brokenRefsCount = 0;

        foreach (var root in rootObjects)
        {
            ScanGameObject(root, ref missingScriptsCount, ref brokenRefsCount);
        }

        Debug.Log(
            $"[NullFinder] Scan complete. Missing scripts: {missingScriptsCount}, broken object references: {brokenRefsCount}");
    }

    private static void ScanGameObject(GameObject go, ref int missingScriptsCount, ref int brokenRefsCount)
    {
        // 1. Missing Scripts
        var components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                Debug.LogWarning($"[NullFinder] Missing script on GameObject '{GetFullPath(go)}'", go);
                missingScriptsCount++;
            }
        }

        // 2. Zerwane referencje w Serializowanych polach
        foreach (var comp in components)
        {
            if (comp == null)
                continue;

            SerializedObject so;
            try
            {
                so = new SerializedObject(comp);
            }
            catch
            {
                // czasem Unity potrafi tu wywalić się na dziwnych obiektach – pomijamy
                continue;
            }

            SerializedProperty prop = so.GetIterator();
            bool enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (prop.propertyType == SerializedPropertyType.ObjectReference)
                {
                    // Zerwana referencja = wartość null, ale InstanceID != 0
                    if (prop.objectReferenceValue == null && prop.objectReferenceInstanceIDValue != 0)
                    {
                        Debug.LogWarning(
                            $"[NullFinder] Broken reference in component {comp.GetType().Name} on '{GetFullPath(go)}', property '{prop.displayName}'",
                            go
                        );
                        brokenRefsCount++;
                    }
                }
            }
        }

        // Rekurencja po dzieciach
        foreach (Transform child in go.transform)
        {
            ScanGameObject(child.gameObject, ref missingScriptsCount, ref brokenRefsCount);
        }
    }

    private static string GetFullPath(GameObject go)
    {
        var path = go.name;
        var current = go.transform.parent;

        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }

    [MenuItem("Tools/swrczk/Find Null References In Project (Prefabs)")]
    public static void FindNullReferencesInPrefabs()
    {
        string[] guids = AssetDatabase.FindAssets("t:GameObject"); // wszystkie prefaby/go jako assety
        int missingScriptsCount = 0;
        int brokenRefsCount = 0;

        for (int i = 0; i < guids.Length; i++)
        {
            string guid = guids[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefabRoot == null)
                continue;

            ScanPrefab(prefabRoot, path, ref missingScriptsCount, ref brokenRefsCount);
        }

        Debug.Log(
            $"[NullFinder] PROJECT scan complete. Missing scripts: {missingScriptsCount}, broken object references: {brokenRefsCount}");
    }

    private static void ScanPrefab(GameObject prefabRoot, string assetPath, ref int missingScriptsCount,
        ref int brokenRefsCount)
    {
        // Wszystkie obiekty w prefabie (również disabled)
        var allTransforms = prefabRoot.GetComponentsInChildren<Transform>(true);

        foreach (var t in allTransforms)
        {
            var go = t.gameObject;
            var components = go.GetComponents<Component>();

            // 1. Missing scripts
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.LogWarning(
                        $"[NullFinder] Missing script in prefab '{assetPath}' on GameObject '{GetFullPath(go, prefabRoot)}'",
                        prefabRoot
                    );
                    missingScriptsCount++;
                }
            }

            // 2. Zerwane referencje w serialized polach
            foreach (var comp in components)
            {
                if (comp == null)
                    continue;

                SerializedObject so;
                try
                {
                    so = new SerializedObject(comp);
                }
                catch
                {
                    // czasem Unity może tu rzucić wyjątek na dziwnych typach
                    continue;
                }

                SerializedProperty prop = so.GetIterator();
                bool enterChildren = true;

                while (prop.NextVisible(enterChildren))
                {
                    enterChildren = false;

                    if (prop.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        // Zerwana referencja: null, ale InstanceID != 0
                        if (prop.objectReferenceValue == null &&
                            prop.objectReferenceInstanceIDValue != 0)
                        {
                            Debug.LogWarning(
                                $"[NullFinder] Broken reference in prefab '{assetPath}' → component {comp.GetType().Name} on '{GetFullPath(go, prefabRoot)}', property '{prop.displayName}'",
                                prefabRoot
                            );
                            brokenRefsCount++;
                        }
                    }
                }
            }
        }
    }

    private static string GetFullPath(GameObject go, GameObject root)
    {
        if (go == root)
            return go.name;

        List<string> names = new List<string>();
        Transform current = go.transform;

        while (current != null)
        {
            names.Add(current.name);
            if (current.gameObject == root)
                break;
            current = current.parent;
        }

        names.Reverse();
        return string.Join("/", names);
    }
}
#endif