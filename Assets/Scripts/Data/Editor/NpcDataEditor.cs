using UnityEditor;

[CustomEditor(typeof(NpcData))]
public class NpcDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NpcData data = (NpcData)target;

        // Standardowy inspector
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Weaknesses Preview:", EditorStyles.boldLabel);

        // Zbieramy słabości
        var parts = new (string label, NpcPartData part)[]
        {
            ("Head", data.Head),
            ("Body", data.Body)
        };

        bool any = false;

        foreach (var entry in parts)
        {
            if (entry.part == null)
                continue;

            if (entry.part.Weakness != null)
            {
                any = true;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"{entry.label}: {entry.part.Weakness.Name}");
                EditorGUILayout.EndVertical();
            }
        }

        if (!any)
        {
            EditorGUILayout.HelpBox("No weaknesses assigned.", MessageType.Info);
        }
    }
}