using UnityEditor;

[CustomEditor(typeof(NpcPartData))]
public class NpcPartDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Pobieramy referencję do obiektu
        NpcPartData data = (NpcPartData)target;

        // Rysujemy standardowy inspector
        DrawDefaultInspector();

        EditorGUILayout.Space();

        // Pokazujemy nazwę Weakness jeśli jest ustawiona
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