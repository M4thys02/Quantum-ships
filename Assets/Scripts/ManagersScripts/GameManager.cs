using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] private TurnManager _turnManager;
    [SerializeField] private BoardManager _boardManager;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private TokenManager _tokenManager;
    [SerializeField] private MeasureManager _measureManager;
    [SerializeField] private UIManager _uiManager;

    private int _currentProbability;
    private int[] _guessedCounts = { 0, 0 };
    private HashSet<Vector3Int>[] _resolvedTiles = { new(), new() };

    private void Start() {
        _currentProbability = (int)PlayerPrefs.GetFloat("SquareSlider", 3);
        _tokenManager.Initialize(_turnManager, _boardManager, _uiManager);

        _inputManager.OnLeftClick += (pos) => _tokenManager.OnTileInteract(pos, false);
        _inputManager.OnRightClick += (pos) => _tokenManager.OnTileInteract(pos, true);

        _uiManager.OnAttackClicked += PlayerAttack;
        _uiManager.OnMeasureClicked += PlayerMeasure;
        _uiManager.OnNextTurnClicked += () => _turnManager.ChangeTurn();

        _turnManager.OnTurnChanged += (prev, curr) => {
            _boardManager.UpdateVisibility(curr);
            _uiManager.UpdateTurnUI(curr);
            _uiManager.ToggleCountersVisibility(curr);
            _uiManager.SetActionButtonsInteractable(true);
            _uiManager.ToggleNextTurnButton(false);
        };

        _uiManager.UpdateProbability(100f / _currentProbability);
        _turnManager.ChangeTurn(); // Start game
        //_uiManager.UpdateTurnUI(_turnManager.CurrentPlayer);
    }

    public void PlayerAttack() {
        _uiManager.SetActionButtonsInteractable(false);
        int attacker = _turnManager.CurrentPlayer;
        int defender = _turnManager.GetOpponent();

        var attackData = _tokenManager.GetActiveSquares();
        var defenderSolution = PlayersSetUps.GetKeyValuePairs(defender);

        foreach (var kvp in defenderSolution) {
            Vector3Int pos = new(kvp.Key.x, kvp.Key.y, 0);
            if (_resolvedTiles[attacker].Contains(pos)) continue;

            if (attackData.TryGetValue(pos, out var list) && list.Count == kvp.Value) {
                _resolvedTiles[attacker].Add(pos);
                _tokenManager.MarkTileAsGuessed(pos, kvp.Value);
                _guessedCounts[attacker] += kvp.Value;
            }
        }

        _uiManager.ToggleNextTurnButton(true);
        CheckWinCondition();
    }

    public void PlayerMeasure() {
        _uiManager.SetActionButtonsInteractable(false);
        int attacker = _turnManager.CurrentPlayer;
        int defender = _turnManager.GetOpponent();
        //Debug.Log($"attacker is: {attacker}, defender is: {defender}");
        Vector2Int tile = PlayersSetUps.GetWeightedRandomTileForPlayer(attacker);

        _measureManager.AddMeasurement(tile, attacker);
        _uiManager.UpdateMeasurementList(attacker, _measureManager.GetPlayerMeasurements(attacker));
        _uiManager.ToggleNextTurnButton(true);
    }

    private void CheckWinCondition() {
        if (_guessedCounts[0] >= _currentProbability || _guessedCounts[1] >= _currentProbability) {
            _uiManager.ToggleNextTurnButton(false);
            // Victory logic...
        }
    }

    public void GameFinished() {
        PlayersSetUps.Cleanup();
        SceneManager.LoadScene("MainMenu");
    }
}