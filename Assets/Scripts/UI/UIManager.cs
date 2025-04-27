using UnityEngine;
using Zenject;

public class UIManager : MonoBehaviour, IInitializable
{
    [Inject] private GameManager _gameManager;
    [Inject] SignalBus _signalBus;
    
    [SerializeField] private GameplayPanel _gameplayPanel;

    public void Initialize()
    {
        _gameplayPanel.Initialize(_gameManager,_signalBus);
    }
}