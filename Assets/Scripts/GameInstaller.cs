using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("References")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private UIManager _uiManager;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<UIManager>().FromInstance(_uiManager).AsSingle();
        Container.BindInterfacesAndSelfTo<GameManager>().FromInstance(_gameManager).AsSingle();
    }
}
