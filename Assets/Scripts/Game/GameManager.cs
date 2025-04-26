using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour, IInitializable
{
    [Header("References")]
    [SerializeField] private GameplayController _gameplayController;
    
    public GameplayController GameplayController => _gameplayController;
    
    [Inject] private GameManager _gameManager;

    public void Initialize()
    {
        _gameplayController.Initialize();
    }
}