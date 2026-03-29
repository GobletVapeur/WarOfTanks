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
        UpdateHUDVisibility();
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
        UpdateHUDVisibility();

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
    private bool IsVisible(TankController from, TankController to)
    {
        Vector3 origin = from.HudAnchor.position;
        Vector3 target = to.HudAnchor.position;

        Vector3 dir = target - origin;

        Ray ray = new Ray(origin, dir.normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, dir.magnitude))
        {
            // si on frappe le tank visible
            if (hit.collider.GetComponentInParent<TankController>() == to)
                return true;

            // sinon mur bloqué
            return false;
        }

        return true;
    }
    
    private void UpdateHUDVisibility()
    {
        TankController active = tanks[currentPlayerIndex];

        for (int i = 0; i < tanks.Length; i++)
        {
            if (i == currentPlayerIndex) {
                tanks[i].HideHUD();
                continue;
                
            }

            bool visible = IsVisible(active, tanks[i]);

            if (visible)
                tanks[i].ShowHUD();
            else
                tanks[i].HideHUD();
        }
    }
}