using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class Cell : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _xMark;
    [SerializeField] private float _xMarkScaleRatio = 0.6f;

 
    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer
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
        Debug.Assert(_xMark != null,"_xMark is not assigned!");
        
        name = $"Cell_{x}_{y}";
        SpriteRenderer.size = new Vector2(size, size);
        _xMark.transform.localScale = Vector3.one * size * _xMarkScaleRatio ;

        ResetMarker();
    }

    public void ToggleMark()
    {
        IsMarked = !IsMarked;
        _xMark.gameObject.SetActive(IsMarked);
    }

    private void ResetMarker()
    {
        IsMarked = false;
        _xMark.gameObject.SetActive(false);
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