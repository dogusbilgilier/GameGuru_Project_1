using UnityEngine;
using Zenject;

public class UIManager : MonoBehaviour, IInitializable
{
    [SerializeField] private GameplayPanel _gameplayPanel;

    [Inject] private GameManager _gameManager;

    public void Initialize()
    {
        _gameplayPanel.Initialize(_gameManager);
    }
}