using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MatchPatternSO))]
public class MatchPatternSOEditor : Editor
{
    private const int CellSize = 25;

    public override void OnInspectorGUI()
    {
        MatchPatternSO patternSO = (MatchPatternSO)target;

        EditorGUILayout.LabelField("Pattern Grid", EditorStyles.boldLabel);

        if (patternSO.PatternRows != null)
        {
            for (int y = 0; y < patternSO.PatternRows.Count; y++)
            {
                var row = patternSO.PatternRows[y];
                EditorGUILayout.BeginHorizontal();
                
                for (int x = 0; x < row.Cells.Count; x++)
                {
                    bool currentValue = row.Cells[x];
                    row.Cells[x] = GUILayout.Toggle(currentValue, "", "Button", GUILayout.Width(CellSize), GUILayout.Height(CellSize));
                }

                if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(CellSize)))
                {
                    row.Cells.Add(false);
                }
                if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(CellSize)))
                {
                    if (row.Cells.Count > 0)
                        row.Cells.RemoveAt(row.Cells.Count - 1);
                }
                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(CellSize)))
                {
                    patternSO.PatternRows.RemoveAt(y);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Row"))
        {
            int columns = patternSO.PatternRows.Count > 0 ? patternSO.PatternRows[0].Cells.Count : 3;
            var newRow = new RowData();
            for (int i = 0; i < columns; i++)
            {
                newRow.Cells.Add(false);
            }
            patternSO.PatternRows.Add(newRow);
        }
        if (GUILayout.Button("Clear Rows"))
        {
            patternSO.PatternRows.Clear();
        }
        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(patternSO);
        }
    }
}
