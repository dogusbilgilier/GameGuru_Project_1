using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameplayController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager _gridManager;
    
    public GridManager GridManager => _gridManager;

    public void Initialize()
    {
        _gridManager.Initialize();
    }
}