using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController[] players;
    [SerializeField] private TankController[] tanks;

    private int currentPlayerIndex;
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogWarning("Multiple GameManager instances detected. Destroying duplicate.");
            Destroy(this);
        }
    }

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

    //private void Update()
    //{
    //    if (Keyboard.current.spaceKey.wasPressedThisFrame)
    //    {
    //        NextTurn();
    //    }
    //}

    public void NextTurn()
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
    }

    private void DeactivatePlayer(int index)
    {
        players[index].SetActive(false);
        tanks[index].EndTurn();
    }
}