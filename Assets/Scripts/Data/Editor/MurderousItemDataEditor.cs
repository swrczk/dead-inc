using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MurderousItemData))]
public class MurderousItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // standardowy inspector
        DrawDefaultInspector();

        var data = (MurderousItemData)target;

        if (data.Weakness != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Weakness Preview", EditorStyles.boldLabel);

            // WeaknessTraitData na pewno dziedziczy z UnityEngine.Object, wiêc ma 'name'
            EditorGUILayout.LabelField("Name:", data.Weakness.name);

            // Jeœli WeaknessTraitData ma w³asne pole Icon, mo¿esz to odkomentowaæ:
            /*
            if (data.Weakness.Icon != null)
            {
                var rect = GUILayoutUtility.GetRect(64, 64, GUILayout.ExpandWidth(false));
                EditorGUI.DrawPreviewTexture(rect, data.Weakness.Icon.texture);
            }
            */
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Brak przypisanego WeaknessTraitData.", MessageType.Info);
        }
    }
}
