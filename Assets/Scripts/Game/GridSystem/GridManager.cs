using UnityEngine;
using UnityEngine.Pool;
using Zenject;

/// <summary>
/// Manages the creation, layout, and interaction of the grid made up of Cells.
/// Handles grid resizing, spacing, padding, and input for marking cells.
/// Also manages object pooling for efficient memory usage.
/// </summary>
public class GridManager : MonoBehaviour
{
    [Inject] SignalBus _signalBus;
    [Inject] private DiContainer _container;

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

    public int CurrentGridSize { get; private set; } = INITIAL_GRID_SIZE;
    private float _spacing;

    private float _topPadding;
    private float _bottomPadding;
    private float _leftPadding;
    private float _rightPadding;

    private float _cellSize;
    private bool _canMarkAnyGrid = true;
    private Vector3 _gridOrigin;

    private Cell[,] _cells;
    public Cell[,] Cells => _cells;

    private Camera _mainCamera;
    private IObjectPool<Cell> _cellPool;

    private Camera MainCamera
    {
        get
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;

            return _mainCamera;
        }
    }

    public void Initialize()
    {
        _cellPool = new ObjectPool<Cell>(CreateCell, OnGetCell, OnReleaseCell, OnDestroyCell);
        CreateGrid(CurrentGridSize);
        _signalBus.Subscribe<MatchStateChangedSignal>(OnMatchStateChanged);
    }


    public void CreateGrid(int size)
    {
        CurrentGridSize = size;
        _cells = new Cell[CurrentGridSize, CurrentGridSize];

        ClearGrid();
        CalculateCellSize();
        CalculateGridsStartPositions();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                var cell = _cellPool.Get();
                cell.Initialize(x, y, _cellSize);
                PositionCell(cell, x, y);

                _cells[x, y] = cell;
            }
        }

        _signalBus.Fire(new MatchGridBuiltSignal());
    }

    private void Update()
    {
        if (_canMarkAnyGrid && Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f;

            HandleClick(worldPosition);
        }
    }

    /// <summary>
    /// Handles click input by converting screen position to world position
    /// and toggling the marked state of the corresponding cell.
    /// </summary>
    private void HandleClick(Vector3 worldPosition)
    {
        //Find clicked grid from position

        float cellTotalSize = _cellSize + _spacing;
        int gridX = Mathf.FloorToInt((worldPosition.x - _gridOrigin.x) / cellTotalSize);
        int gridY = Mathf.FloorToInt((worldPosition.y - _gridOrigin.y) / cellTotalSize);

        if (gridX >= 0 && gridX < CurrentGridSize && gridY >= 0 && gridY < CurrentGridSize)
        {
            Cell clickedCell = _cells[gridX, gridY];
            clickedCell.ToggleMark();
        }
    }

    /// <summary>
    /// Calculates the starting world position of the grid
    /// based on camera view, paddings, and available space.
    /// </summary>
    private void CalculateGridsStartPositions()
    {
        var available = CalculateAvailableWidthAndHeight();
        float screenHeight = 2f * MainCamera.orthographicSize;
        float screenWidth = screenHeight * MainCamera.aspect;

        Vector3 cameraCenter = MainCamera.transform.position;
        float screenBottom = cameraCenter.y - screenHeight * 0.5f;
        float screenLeft = cameraCenter.x - screenWidth * 0.5f;

        float gridTotalSize = ((CurrentGridSize - 1) * _spacing) + (CurrentGridSize * _cellSize);

        //Centralize grid according to camera and paddings
        float startX = screenLeft + _leftPadding + (available.width - gridTotalSize) * 0.5f;
        float startY = screenBottom + _bottomPadding + (available.height - gridTotalSize) * 0.5f;

        _gridOrigin = new Vector3(startX, startY, 0f);
    }

    private void CalculateCellSize()
    {
        var available = CalculateAvailableWidthAndHeight();
        float roughCellSize = Mathf.Min(available.width, available.height) / CurrentGridSize;

        _spacing = roughCellSize * _spacingPercent;

        float availableWidth = available.width - (_spacing * (CurrentGridSize - 1));
        float availableHeight = available.height - (_spacing * (CurrentGridSize - 1));

        // Set cell size according to min sized screen axis
        _cellSize = Mathf.Min(availableWidth, availableHeight) / CurrentGridSize;
    }

    /// <summary>
    /// Calculates the usable screen width and height
    /// after applying padding percentages.
    /// </summary>
    /// <returns>Tuple of available width and height.</returns>
    private (float width, float height) CalculateAvailableWidthAndHeight()
    {
        //Calculate total visible area
        float screenHeight = 2f * MainCamera.orthographicSize;
        float screenWidth = screenHeight * MainCamera.aspect;

        //Convert padding percents to real distances 
        _topPadding = screenHeight * _topPaddingPercent;
        _bottomPadding = screenHeight * _bottomPaddingPercent;
        _leftPadding = screenWidth * _leftPaddingPercent;
        _rightPadding = screenWidth * _rightPaddingPercent;

        //Calculate usable area
        float usableHeight = screenHeight - _topPadding - _bottomPadding;
        float usableWidth = screenWidth - _leftPadding - _rightPadding;

        return (usableWidth, usableHeight);
    }

    /// <summary>
    /// Positions a cell within the grid based on its grid coordinates,
    /// cell size, spacing, and grid origin.
    /// Also adjusts the SpriteRenderer size accordingly.
    /// </summary>
    private void PositionCell(Cell cell, int x, int y)
    {
        float halfCellSize = _cellSize * 0.5f;
        float cellWithSpacing = _cellSize + _spacing;

        float xPos = _gridOrigin.x + x * cellWithSpacing + halfCellSize;
        float yPos = _gridOrigin.y + y * cellWithSpacing + halfCellSize;

        Vector3 position = new Vector3(xPos, yPos, 0f);

        cell.transform.localPosition = position;

        var spriteRenderer = cell.SpriteRenderer;
        if (spriteRenderer != null)
        {
            spriteRenderer.size = new Vector2(_cellSize, _cellSize);
        }
    }

    /// <summary>
    /// Deactivates all cells currently inside the grid by releasing them back to the object pool.
    /// </summary>
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

    private void OnMatchStateChanged(MatchStateChangedSignal args)
    {
        _canMarkAnyGrid = args.IsMatchesInProgress == false;
    }

    #region CellPool

    /// <summary>
    /// Creates a new Cell instance from the prefab,
    /// injects its dependencies manually using the Di Container,
    /// </summary>
    private Cell CreateCell()
    {
        Cell cell = Instantiate(_cellPrefab, _cellContainer);
        _container.Inject(cell);

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

    /// <summary>
    /// Called when a cell is retrieved from the pool.
    /// Reactivates the cell's GameObject.
    /// </summary>
    private void OnGetCell(Cell cell)
    {
        cell.gameObject.SetActive(true);
    }

    #endregion
}

public struct MatchGridBuiltSignal
{
}