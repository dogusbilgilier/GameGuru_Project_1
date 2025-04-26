using UnityEngine;
using Zenject;

public class GameplayController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private MatchPatternSO[] _matchPatterns;
    public GridManager GridManager => _gridManager;
    private MatchManager _matchManager;
    private SignalBus _signalBus;

    public void Initialize(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _gridManager.Initialize();
        _matchManager = new MatchManager(_gridManager, signalBus, _matchPatterns);
    }
}