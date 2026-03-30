using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController[] players;
    [SerializeField] private TankController[] tanks;
    [SerializeField] private HUDManager hud;
    [SerializeField] private MinimapManager minimap; 
    [SerializeField] private CameraFollowTurret mainCamera;

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
    private void Update()
    {
        

        UpdateVisibilityState();
    }

    private void NextTurn()
    {
        DeactivatePlayer(currentPlayerIndex);
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
  
        ActivatePlayer(currentPlayerIndex);
    }

    private void ActivatePlayer(int index)
    {
        players[index].SetActive(true);
        tanks[index].BeginTurn();
        hud.SetActiveTank(tanks[index]);
        mainCamera.SetTarget(tanks[index]);
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

    private void UpdateVisibilityState()
    {
        TankController active = tanks[currentPlayerIndex];

        foreach (var tank in tanks)
        {
            // Le joueur actif est toujours visible
            if (tank == active)
            {
                tank.gameObject.SetActive(true);

                tank.HideHUD();

                tank.SetMinimapVisible(true);
                tank.SetMinimapAsAlly();

                continue;
            }

            bool visible = IsVisible(active, tank);

            // Visibilité dans la scène
            tank.gameObject.SetActive(visible);

            // Si le tank est visible, on peut afficher HUD + minimap
            if (visible)
            {
                tank.ShowHUD();
                tank.SetMinimapVisible(true);
                tank.SetMinimapAsEnemy();
            }
            else
            {
                tank.HideHUD();
                tank.SetMinimapVisible(false);
            }
        }
    }
}