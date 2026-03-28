using UnityEngine;

public class PlayerController : MonoBehaviour
{ 
    private TankController tank;

    private TankControls controls;
    private bool isActive = true;
    
    public void Initialize(TankController assignedTank)
    {
        tank = assignedTank;
    }

    private void Awake()
    {
        controls = new TankControls();
        controls.Gameplay.Move.performed += ctx => tank.moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => tank.moveInput = Vector2.zero;
        controls.Gameplay.Aim.performed += ctx => tank.SetAimInput(ctx.ReadValue<Vector2>());
        controls.Gameplay.Aim.canceled += ctx => tank.SetAimInput(Vector2.zero);
        controls.Gameplay.Fire.performed += ctx => tank.fire();
        controls.Gameplay.Enable();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        if (controls != null)
            controls.Gameplay.Disable();
    }

    private void Update()
    {
        if (tank == null)
        {
            Debug.LogError(name + " has no tank assigned.");
            return;
        }
        
        if (isActive)
        {
            //tank?.SetMoveInput(Vector2.zero);
            // tank?.SetAimInput(Vector2.zero);
            controls.Gameplay.Enable();
            return;
        }

        ////Vector2 moveInput = controls.Gameplay.Move.ReadValue<Vector2>();
        //Vector2 aimInput = controls.Gameplay.Aim.ReadValue<Vector2>();

        ////tank?.SetMoveInput(moveInput);
        //tank?.SetAimInput(aimInput);
    }

    public void SetActive(bool active)
    {
        isActive = active;

        tank?.SetControlEnabled(active);

        if (!active)
        {
            //tank?.SetMoveInput(Vector2.zero);
            //tank?.SetAimInput(Vector2.zero);
            controls.Gameplay.Disable();
        }
    }
}
