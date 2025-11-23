using UnityEditor;

[CustomEditor(typeof(MurderousItemData))]
public class MurderousItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var data = (MurderousItemData)target;

        if (data.Weakness != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Weakness Preview", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Name:", data.Weakness.name);
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Brak przypisanego WeaknessTraitData.", MessageType.Info);
        }
    }
}