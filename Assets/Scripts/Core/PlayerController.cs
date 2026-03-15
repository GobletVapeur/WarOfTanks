using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TankController tank;
    [SerializeField] private TurretController turret;

    private TankControls controls;
    private bool isActive = true;

    private void Awake()
    {
        controls = new TankControls();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void Update()
    {
        if (!isActive)
            return;

        Vector2 moveInput = controls.Gameplay.Move.ReadValue<Vector2>();
        Vector2 aimInput = controls.Gameplay.Aim.ReadValue<Vector2>();

        if (tank != null)
            tank.SetMoveInput(moveInput);

        if (turret != null)
            turret.SetAimInput(aimInput);
    }

    public void SetActive(bool active)
    {
        isActive = active;

        if (!active)
        {
            tank?.SetMoveInput(Vector2.zero);
            turret?.SetAimInput(Vector2.zero);
        }
    }
}
