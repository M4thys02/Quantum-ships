using System;
using UnityEngine;

public class TurnManager : MonoBehaviour {
    public int CurrentPlayer { get; private set; } = 0;
    public event Action<int, int> OnTurnChanged;

    public void ChangeTurn() {
        int previousPlayer = CurrentPlayer;
        CurrentPlayer = (CurrentPlayer == 0) ? 1 : 0;
        //Debug.Log($"Turn changed: {previousPlayer} -> {CurrentPlayer}");
        OnTurnChanged?.Invoke(previousPlayer, CurrentPlayer);
    }

    public int GetOpponent() => CurrentPlayer ^ 1;
}