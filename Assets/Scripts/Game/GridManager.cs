using Game;
using UnityEngine;
using UnityEngine.Pool;

public class GridManager : MonoBehaviour
{
    private const int INITIAL_GRID_SIZE = 5;

    [Header("References")]
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private Transform _cellContainer;

    [Header("Spacing")]
    [Range(0f, 1f)]
    [SerializeField] private float _spacingPercent = 0.1f;

    [Header("Paddings")]
    [Range(0f, 1f)]
    [SerializeField] private float _topPaddingPercent = 0.1f;
    [Range(0f, 1f)]
    [SerializeField] private float _bottomPaddingPercent = 0.1f;
    [Range(0f, 1f)]
    [SerializeField] private float _leftPaddingPercent = 0.05f;
    [Range(0f, 1f)]
    [SerializeField] private float _rightPaddingPercent = 0.05f;

    private int _gridSize = INITIAL_GRID_SIZE;
    private float _spacing;
    private float _topPadding;
    private float _bottomPadding;
    private float _leftPadding;
    private float _rightPadding;

    private Camera _mainCamera;

    private Camera MainCamera
    {
        get
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;

            return _mainCamera;
        }
    }

    private float _cellSize;
    private IObjectPool<Cell> _cellPool;

    public void Initialize()
    {
        _cellPool = new ObjectPool<Cell>(CreateCell, OnGetCell, OnReleaseCell, OnDestroyCell);
        CreateGrid(_gridSize);
    }

    public void CreateGrid(int size)
    {
        _gridSize = size;

        ClearGrid();
        CalculateCellSize();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                var cell = _cellPool.Get();
                cell.Initialize(x, y);
                cell.name = $"Cell_{x}_{y}";
                PositionCell(cell, x, y);
            }
        }
    }

    private void CalculateCellSize()
    {
        var usable = CalculateAvailableWidthAndHeight();
        float roughCellSize = Mathf.Min(usable.width, usable.height) / _gridSize;
        _spacing = roughCellSize * _spacingPercent;
        float availableWidth = usable.width - (_spacing * (_gridSize - 1));
        float availableHeight = usable.height - (_spacing * (_gridSize - 1));
        _cellSize = Mathf.Min(availableWidth, availableHeight) / _gridSize;
    }

    private (float width, float height) CalculateAvailableWidthAndHeight()
    {
        float screenHeight = 2f * MainCamera.orthographicSize;
        float screenWidth = screenHeight * MainCamera.aspect;

        _topPadding = screenHeight * _topPaddingPercent;
        _bottomPadding = screenHeight * _bottomPaddingPercent;
        _leftPadding = screenWidth * _leftPaddingPercent;
        _rightPadding = screenWidth * _rightPaddingPercent;

        float usableHeight = screenHeight - _topPadding - _bottomPadding;
        float usableWidth = screenWidth - _leftPadding - _rightPadding;

        return (usableWidth, usableHeight);
    }

    private void PositionCell(Cell cell, int x, int y)
    {
        var usable = CalculateAvailableWidthAndHeight();
        float screenHeight = 2f * MainCamera.orthographicSize;
        float screenWidth = screenHeight * MainCamera.aspect;

        Vector3 cameraCenter = MainCamera.transform.position;
        float screenBottom = cameraCenter.y - screenHeight * 0.5f;
        float screenLeft = cameraCenter.x - screenWidth * 0.5f;

        float gridTotalSize = ((_gridSize - 1) * _spacing) + (_gridSize * _cellSize);

        float startX = screenLeft + _leftPadding + (usable.width - gridTotalSize) * 0.5f;
        float startY = screenBottom + _bottomPadding + (usable.height - gridTotalSize) * 0.5f;

        float halfCellSize = _cellSize * 0.5f;
        float cellWithSpacing = _cellSize + _spacing;

        float xPos = startX + x * cellWithSpacing + halfCellSize;
        float yPos = startY + y * cellWithSpacing + halfCellSize;

        Vector3 position = new Vector3(xPos, yPos, 0f);

        cell.transform.localPosition = position;

        var spriteRenderer = cell.SpriteRenderer;
        if (spriteRenderer != null)
        {
            spriteRenderer.size = new Vector2(_cellSize, _cellSize);
        }
    }

    private void ClearGrid()
    {
        for (int i = 0; i < _cellContainer.childCount; i++)
        {
            var child = _cellContainer.GetChild(i);
            if (child.TryGetComponent(out Cell cell))
            {
                if (cell.gameObject.activeSelf)
                    cell.ReleaseFromPool();
            }
        }
    }

    #region CellPool

    private Cell CreateCell()
    {
        var cell = Instantiate(_cellPrefab, _cellContainer);
        cell.AssignToPool(_cellPool);

        return cell;
    }

    private void OnDestroyCell(Cell cell)
    {
        cell.ReleaseFromPool();
    }

    private void OnReleaseCell(Cell cell)
    {
        cell.gameObject.SetActive(false);
    }

    private void OnGetCell(Cell cell)
    {
        cell.gameObject.SetActive(true);
    }

    #endregion
}