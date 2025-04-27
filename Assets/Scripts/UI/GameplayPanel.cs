using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameplayPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _generateGridButton;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private SignalBus _signalBus;
    private GameManager _gameManager;

    public void Initialize(GameManager gameManager, SignalBus signalBus)
    {
        _gameManager = gameManager;
        _signalBus = signalBus;

        _generateGridButton.onClick.RemoveAllListeners();
        _generateGridButton.onClick.AddListener(OnGenerateGridButtonClicked);

        _signalBus.Subscribe<MatchStateChangedSignal>(OnMatchStateChanged);
        _signalBus.Subscribe<MatchScoreChangedSignal>(OnScoreChanged);
    }

    private int GetInputNumber()
    {
        string currentInput = _inputField.text;

        if (int.TryParse(currentInput, out int inputNumber))
            return inputNumber;

        Debug.LogError($"Invalid input: {currentInput}");
        return -1;
    }

    private void OnGenerateGridButtonClicked()
    {
        int size = GetInputNumber();

        if (size > 0)
            _gameManager.GameplayController.GridManager.CreateGrid(size);
    }

    private void OnMatchStateChanged(MatchStateChangedSignal args)
    {
        _generateGridButton.interactable = args.IsMatchesInProgress == false;
    }

    private void OnScoreChanged(MatchScoreChangedSignal args)
    {
        _scoreText.SetText($"Score:{args.Score}");
    }
}