using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameFlowManager))]
public class GameFlowManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Show All"))
        {
            ShowAll();
        }

        if (GUILayout.Button("Assign Random Paths"))
        {
            AssignRandomPaths();
        }

        EditorGUILayout.Space();
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Weakness Summary per Stage", EditorStyles.boldLabel);

        var manager = (GameFlowManager)target;
        var flow = manager.GameplayFlow;

        if (flow == null || flow.Stages == null || flow.Stages.Count == 0)
        {
            EditorGUILayout.HelpBox("Brak zdefiniowanych Stage'y.", MessageType.Info);
            return;
        }

        for (int i = 0; i < flow.Stages.Count; i++)
        {
            var stage = flow.Stages[i];
            if (stage == null)
                continue;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"Stage {i}", EditorStyles.boldLabel);

            Dictionary<string, int> weaknessCount = new Dictionary<string, int>();

            var npcSets = stage.AvailableNpcs;
            if (npcSets != null)
            {
                foreach (var npcSet in npcSets)
                {
                    if (npcSet == null || npcSet.NpcType == null)
                        continue;

                    AddWeakness(weaknessCount, npcSet.NpcType.Head, npcSet.Amount);
                    AddWeakness(weaknessCount, npcSet.NpcType.Body, npcSet.Amount);
                }
            }

            if (weaknessCount.Count == 0)
            {
                EditorGUILayout.HelpBox("Brak weaknessów w tym Stage.", MessageType.Info);
            }
            else
            {
                foreach (var kvp in weaknessCount)
                {
                    EditorGUILayout.LabelField($"{kvp.Key}: {kvp.Value}");
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
    }

    private void AddWeakness(Dictionary<string, int> dict, NpcPartData part, int amount)
    {
        if (part == null || part.Weakness == null)
            return;

        string key = string.IsNullOrEmpty(part.Weakness.Name)
            ? "Unnamed Weakness"
            : part.Weakness.Name;

        if (!dict.ContainsKey(key))
            dict[key] = 0;

        dict[key] += amount;
    }

    private void ShowAll()
    {
        // Jeśli chcesz rozszerzyć WSZYSTKO w tym komponencie:
        // ExpandAll(serializedObject);

        // Jeśli chcesz tylko gameplayFlow i jego wnętrze:
        SerializedProperty gameplayFlowProp = serializedObject.FindProperty("gameplayFlow");
        if (gameplayFlowProp != null)
        {
            SetExpandedRecursively(gameplayFlowProp, true);
        }
    }

    private void ExpandAll(SerializedObject obj)
    {
        var prop = obj.GetIterator();
        bool enterChildren = true;

        while (prop.NextVisible(enterChildren))
        {
            prop.isExpanded = true;
            enterChildren = true;
        }
    }

    private void AssignRandomPaths()
    {
        var manager = (GameFlowManager)target;

        // Pobierz wszystkie ścieżki ze sceny
        var allPaths = GameObject.FindObjectsOfType<WaypointPath>();
        if (allPaths.Length == 0)
        {
            Debug.LogWarning("[GameFlow Editor] No WaypointPath objects found in scene!");
            return;
        }

        var gameplayFlow = manager.GameplayFlow;
        if (gameplayFlow == null || gameplayFlow.Stages == null)
        {
            Debug.LogWarning("[GameFlow Editor] GameplayFlow or Stages is null!");
            return;
        }

        Undo.RecordObject(manager, "Assign Random Paths");

        foreach (var stage in gameplayFlow.Stages)
        {
            if (stage.AvailableNpcs == null) continue;

            foreach (var npcSet in stage.AvailableNpcs)
            {
                if (npcSet != null && npcSet.shoppingPath == null)
                {
                    npcSet.shoppingPath = allPaths[Random.Range(0, allPaths.Length)];
                }
            }
        }

        EditorUtility.SetDirty(manager);
        Debug.Log("[GameFlow Editor] Random paths assigned successfully.");
    }

    private void SetExpandedRecursively(SerializedProperty property, bool expand)
    {
        SerializedProperty current = property.Copy();
        SerializedProperty end = property.GetEndProperty();

        current.isExpanded = expand;

        // Schodzimy po wszystkich dzieciach aż do endProperty
        while (current.NextVisible(true) && !SerializedProperty.EqualContents(current, end))
        {
            current.isExpanded = expand;
        }
    }
}