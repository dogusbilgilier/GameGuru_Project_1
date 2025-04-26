using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MatchManager
{
    private MatchPatternSO[] _matchPatterns;
    private SignalBus _signalBus;
    private GridManager _gridManager;

    List<List<Cell>> _matchedCellList = new List<List<Cell>>();
    List<Cell> _tempMatchedCells = new List<Cell>();

    public MatchManager(GridManager gridManager, SignalBus signalBus, MatchPatternSO[] matchPatterns)
    {
        _matchPatterns = matchPatterns;
        _signalBus = signalBus;
        _gridManager = gridManager;

        _signalBus.Subscribe<CellMarkedSignal>(OnCellMarkedStateChanged);
    }

    /// <summary>
    /// Manages the matching logic between cells and predefined patterns.
    /// </summary>
    public void CheckForMatches()
    {
        for (int x = 0; x < _gridManager.CurrentGridSize; x++)
        {
            for (int y = 0; y < _gridManager.CurrentGridSize; y++)
            {
                var cell = _gridManager.Cells[x, y];

                Debug.Assert(cell != null);

                foreach (var patternSO in _matchPatterns)
                {
                    var patternMatrix = patternSO.GetMatrix();
                    if (DoesPatternMatchFromCell(cell, patternMatrix))
                    {
                        //Match Found!
                        //Debug.Log($"Match found starting at ({cell.Coordinates.x}, {cell.Coordinates.y})!");
                    }
                }
            }
        }

        ClearMatchedCells();
    }

    /// <summary>
    ///  Checks if a specific pattern matches starting from the given cell.
    ///  Returns true if the pattern fits based on current cells.
    /// </summary>
    private bool DoesPatternMatchFromCell(Cell startCell, bool[,] pattern)
    {
        _tempMatchedCells.Clear();
        int patternWidth = pattern.GetLength(0);
        int patternHeight = pattern.GetLength(1);

        for (int x = 0; x < patternWidth; x++)
        {
            for (int y = 0; y < patternHeight; y++)
            {
                if (pattern[x, y])
                {
                    int checkX = startCell.Coordinates.x + x;
                    int checkY = startCell.Coordinates.y + y;

                    if (checkX >= _gridManager.CurrentGridSize || checkY >= _gridManager.CurrentGridSize)
                        return false;

                    Cell cell = _gridManager.Cells[checkX, checkY];
                    if (cell == null || !cell.IsMarked)
                        return false;

                    _tempMatchedCells.Add(cell);
                }
            }
        }

        _matchedCellList.Add(new List<Cell>(_tempMatchedCells));
        return true;
    }

    private void ClearMatchedCells()
    {
        foreach (List<Cell> cells in _matchedCellList)
        {
            foreach (Cell cell in cells)
            {
                if (cell.IsMarked)
                {
                    //Sent silent to avoid stack overflow
                    cell.ToggleMark(true);
                }
            }
        }

        _tempMatchedCells.Clear();
        _matchedCellList.Clear();
    }
    
    private void OnCellMarkedStateChanged(CellMarkedSignal args)
    {
        CheckForMatches();
    }
}