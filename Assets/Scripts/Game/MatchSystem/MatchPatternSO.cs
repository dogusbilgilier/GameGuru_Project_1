using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MatchPatternSO", menuName = "Match3/MatchPatternSO")]
public class MatchPatternSO : ScriptableObject
{
    // List of rows representing the pattern 
    public List<RowData> PatternRows = new List<RowData>();

    /// <summary>
    /// Converts the list-based pattern into a 2D boolean array (matrix) for easier processing.
    /// </summary>
    /// <returns>A 2D boolean array representing the pattern (width x height).</returns>
    public bool[,] GetMatrix()
    {
        int height = PatternRows.Count;
        int width = height > 0 ? PatternRows[0].Cells.Count : 0;

        bool[,] matrix = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            var row = PatternRows[y];
            for (int x = 0; x < width; x++)
            {
                matrix[x, y] = row.Cells[x];
            }
        }

        return matrix;
    }
}

[Serializable]
public class RowData
{
    public List<bool> Cells = new List<bool>();
}