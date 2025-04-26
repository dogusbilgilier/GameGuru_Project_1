using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _generateGridButton;

    private GameManager _gameManager;

    public void Initialize(GameManager gameManager)
    {
        _gameManager = gameManager;

        _generateGridButton.onClick.RemoveAllListeners();
        _generateGridButton.onClick.AddListener(OnGenerateGridButtonClicked);
    }

    private void OnGenerateGridButtonClicked()
    {
        int size = GetInputNumber();

        if (size > 0)
            _gameManager.GameplayController.GridManager.CreateGrid(size);
    }

    private int GetInputNumber()
    {
        string currentInput = _inputField.text;

        if (int.TryParse(currentInput, out int inputNumber))
            return inputNumber;

        Debug.LogError($"Invalid input: {currentInput}");
        return -1;
    }
}