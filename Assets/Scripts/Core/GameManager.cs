using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController[] players;
    [SerializeField] private TankController[] tanks;
    [SerializeField] private HUDManager hud;

    private int currentPlayerIndex;

    private void Start()
    {
        if (players.Length != tanks.Length)
        {
            Debug.LogError("Players and Tanks count mismatch!");
            return;
        }

        for (int i = 0; i < players.Length; i++)
        {
            players[i].Initialize(tanks[i]);
            players[i].SetActive(false);
            tanks[i].EndTurn();
        }

        currentPlayerIndex = 0;
        ActivatePlayer(currentPlayerIndex);
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            NextTurn();
            CheckEndGame();
        }
    }

    private void NextTurn()
    {
        // Fin du tour actuel
        DeactivatePlayer(currentPlayerIndex);

        // Prochain joueur
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;

        // Début du tour suivant
        ActivatePlayer(currentPlayerIndex);
    }

    private void ActivatePlayer(int index)
    {
        players[index].SetActive(true);
        tanks[index].BeginTurn();
        hud.SetActiveTank(tanks[index]);
    }

    private void DeactivatePlayer(int index)
    {
        players[index].SetActive(false);
        tanks[index].EndTurn();
    }
    
    private void CheckEndGame()
    {
        int aliveCount = 0;

        foreach (var tank in tanks)
        {
            if (tank.Stats.IsAlive)
                aliveCount++;
        }

        if (aliveCount <= 1)
        {
            EndGame();
        }
    }
    
    private void EndGame()
    {
        SceneManager.LoadScene("EndScreen");
    }
}