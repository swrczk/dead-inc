using UnityEditor;

[CustomEditor(typeof(MurderousItemData))]
public class MurderousItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MurderousItemData data = (MurderousItemData) target;
        DrawDefaultInspector();

        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("AffectsOn Preview:", EditorStyles.boldLabel);

        if (data.AffectsOn == null || data.AffectsOn.Count == 0)
        {
            EditorGUILayout.HelpBox("List is empty.", MessageType.Info);
        }
        else
        {
            for (int i = 0; i < data.AffectsOn.Count; i++)
            {
                var weakness = data.AffectsOn[i];

                if (weakness == null)
                {
                    EditorGUILayout.LabelField($"[{i}] NULL");
                    continue;
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"[{i}] {weakness.Name}");
                EditorGUILayout.EndVertical();
            }
        }
    }
}