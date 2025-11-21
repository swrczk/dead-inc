using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameplayFlow))]
public class GameplayFlowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Standardowy inspector (lista Stage’y itd.)
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Weakness Summary per Stage", EditorStyles.boldLabel);

        var flow = (GameplayFlow)target;
        if (flow.Stages == null || flow.Stages.Count == 0)
        {
            EditorGUILayout.HelpBox("Brak zdefiniowanych Stage'y.", MessageType.Info);
            return;
        }

        // Dla każdego Stage robimy podsumowanie
        for (int i = 0; i < flow.Stages.Count; i++)
        {
            var stage = flow.Stages[i];
            if (stage == null)
                continue;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"Stage {i}", EditorStyles.boldLabel);

            // słownik: nazwa weakness -> suma
            Dictionary<string, int> weaknessCount = new Dictionary<string, int>();

            if (stage.AvailableNpcs != null)
            {
                foreach (var npcSet in stage.AvailableNpcs)
                {
                    if (npcSet == null || npcSet.NpcType == null)
                        continue;

                    // tutaj zakładam strukturę NpcData taką jak w Twoim innym edytorze:
                    // public NpcPartData Hair, Chest, Legs;
                    AddWeakness(weaknessCount, npcSet.NpcType.Hair, npcSet.Amount);
                    AddWeakness(weaknessCount, npcSet.NpcType.Chest, npcSet.Amount);
                    AddWeakness(weaknessCount, npcSet.NpcType.Legs, npcSet.Amount);
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

        // mnożymy przez Amount, żeby uwzględnić liczbę NPC tego typu w Stage’u
        dict[key] += amount;
    }
}
