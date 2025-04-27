using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

public class Cell : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    
    [Header("References")]
    [SerializeField] private GameObject _xMark;
    [SerializeField] private float _xMarkScaleRatio = 0.6f;

    [Header("References - Visual Feedback")]
    [SerializeField] private Color _colorOnMatch;
    [SerializeField] private float _scaleOverflowAmount = 0.2f;
    public Vector2Int Coordinates { get; private set; } // The grid coordinates (X, Y) of this cell.

    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer // Cached reference to the SpriteRenderer  
    {
        get
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            return _spriteRenderer;
        }
    }

    private IObjectPool<Cell> _assignedPool;

    public bool IsMarked { get; private set; }

    public void Initialize(int x, int y, float size)
    {
        Debug.Assert(SpriteRenderer != null, "SpriteRenderer can not found!");
        Debug.Assert(_xMark != null, "_xMark is not assigned!");

        Coordinates = new Vector2Int(x, y);

        name = $"Cell_{x}_{y}";
        SpriteRenderer.size = new Vector2(size, size);
        _xMark.transform.localScale = Vector3.one * size * _xMarkScaleRatio;

        if (IsMarked)
            ToggleMark(true);
    }

    public Sequence ChanceColorInSeconds(float seconds)
    {
        float waitInColor = 0.2f;
        Color startColor = SpriteRenderer.color;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Join(_spriteRenderer.DOColor(_colorOnMatch, seconds));
        sequence.AppendInterval(waitInColor);
        sequence.OnComplete(() => { SpriteRenderer.color = startColor; });
        return sequence;
    }

    public Sequence PunchScaleInSeconds(float seconds)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(_xMark.transform.DOPunchScale(Vector3.one * _scaleOverflowAmount, seconds, vibrato: 1));
        return sequence;
    }

    /// <param name="isSilent">If true, prevents firing a signal when toggling the mark.</param>
    public void ToggleMark(bool isSilent = false)
    {
        IsMarked = !IsMarked;
        _xMark.gameObject.SetActive(IsMarked);

        if (isSilent)
            return;

        _signalBus.Fire(new CellMarkedSignal
        {
            IsMarked = this.IsMarked,
            Cell = this
        });
    }


    public void AssignToPool(IObjectPool<Cell> assignedPool)
    {
        _assignedPool = assignedPool;
    }

    public void ReleaseFromPool()
    {
        Debug.Assert(_assignedPool != null);
        _assignedPool.Release(this);
    }
}

/// <summary>
/// Signal used to notify when a cell's marked state changes.
/// </summary>
public struct CellMarkedSignal
{
    public Cell Cell;
    public bool IsMarked;
}