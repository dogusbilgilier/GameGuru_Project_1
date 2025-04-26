using UnityEngine;
using UnityEngine.Pool;

public class Cell : MonoBehaviour
{
    private IObjectPool<Cell> _assignedPool;
    private Vector2 _coords;

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

    public void Initialize(int x, int y)
    {
        _coords = new Vector2(x, y);
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