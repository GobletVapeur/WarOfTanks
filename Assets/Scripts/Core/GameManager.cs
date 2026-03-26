using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController[] players;
    [SerializeField] private TankController[] tanks;

    private int currentPlayerIndex;

    private void Start()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].Initialize(tanks[i]);
            players[i].SetActive(false);
        }

        currentPlayerIndex = 0;
        players[currentPlayerIndex].SetActive(true);
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            NextTurn();
        }
    }

    private void NextTurn()
    {
        players[currentPlayerIndex].SetActive(false);

        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Length)
            currentPlayerIndex = 0;

        players[currentPlayerIndex].SetActive(true);
    }
}