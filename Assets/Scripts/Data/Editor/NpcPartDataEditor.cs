using UnityEditor;

[CustomEditor(typeof(NpcPartData))]
public class NpcPartDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NpcPartData data = (NpcPartData)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (data.Weakness != null)
        {
            EditorGUILayout.LabelField("Weakness Preview:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Name: ", data.Weakness.Name);
        }
        else
        {
            EditorGUILayout.HelpBox("No Weakness assigned.", MessageType.Info);
        }
    }
}