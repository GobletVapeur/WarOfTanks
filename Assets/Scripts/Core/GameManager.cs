using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TankController[] tanks;

    public event Action<TankController> TurnStarted;
    public event Action<TankController> TurnEnded;

    private int activeTankIndex = -1;

    public TankController ActiveTank =>
        activeTankIndex >= 0 && activeTankIndex < tanks.Length
            ? tanks[activeTankIndex]
            : null;

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        if (tanks == null || tanks.Length == 0)
        {
            Debug.LogError("GameManager requires at least one TankController.", this);
            return;
        }

        for (int i = 0; i < tanks.Length; i++)
        {
            if (tanks[i] == null)
            {
                Debug.LogError($"GameManager tank slot {i} is not assigned.", this);
                return;
            }

            tanks[i].EndTurn();
        }

        activeTankIndex = 0;
        BeginTurn(activeTankIndex);
    }

    public void EndTurn()
    {
        if (tanks == null || tanks.Length == 0 || activeTankIndex < 0)
        {
            return;
        }

        TankController previousTank = tanks[activeTankIndex];
        previousTank.EndTurn();
        TurnEnded?.Invoke(previousTank);

        activeTankIndex = (activeTankIndex + 1) % tanks.Length;
        BeginTurn(activeTankIndex);
    }

    private void BeginTurn(int tankIndex)
    {
        TankController currentTank = tanks[tankIndex];
        currentTank.BeginTurn();
        TurnStarted?.Invoke(currentTank);
    }
}
