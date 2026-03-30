using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController[] players;
    [SerializeField] private TankController[] tanks;
    [SerializeField] private HUDManager hud;
    [SerializeField] private MinimapManager minimap; // ← AJOUTÉ

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
        CheckEndGame();
        // Fin du tour actuel
        DeactivatePlayer(currentPlayerIndex);
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        UpdateHUDVisibility();
        ActivatePlayer(currentPlayerIndex);
    }

    private void ActivatePlayer(int index)
    {
        players[index].SetActive(true);
        tanks[index].BeginTurn();
        hud.SetActiveTank(tanks[index]);
        minimap.SetTarget(tanks[index].transform);
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

    private bool IsVisible(TankController from, TankController to)
    {
        Vector3 origin = from.HudAnchor.position;
        Vector3 target = to.HudAnchor.position;

        Vector3 dir = target - origin;

        Ray ray = new Ray(origin, dir.normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, dir.magnitude))
        {
            if (hit.collider.GetComponentInParent<TankController>() == to)
                return true;

            return false;
        }

        return true;
    }

    private void UpdateHUDVisibility()
    {
        TankController active = tanks[currentPlayerIndex];

        // 1. Reset tout
        foreach (var tank in tanks)
        {
            tank.SetMinimapVisible(false);
        }

        // 2. Actif (toujours visible + vert)
        active.SetMinimapVisible(true);
        active.SetMinimapAsAlly();

        // 3. Ennemis
        foreach (var tank in tanks)
        {
            if (tank == active)
                continue;

            bool visible = IsVisible(active, tank);

            if (visible)
            {
                tank.SetMinimapVisible(true);
                tank.SetMinimapAsEnemy();
            }
        }
    }
}