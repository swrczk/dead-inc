using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(GameFlowManager))]
public class GameFlowManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Rysujemy normalny inspector GameFlowManagera
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
}
