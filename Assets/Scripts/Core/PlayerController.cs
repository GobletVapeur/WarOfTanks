using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{ 
    private TankController tank;

    //private TankControls controls;
    private bool isActive = true;
    private PlayerInput playerInput;


    public void Initialize(TankController assignedTank)
    {
        tank = assignedTank;
    }

    private void Start()
    {
        //controls = new TankControls();
        playerInput = gameObject.GetComponent<PlayerInput>();
        // Let PlayerInput automatically switch control schemes when the player
        // changes device. Make sure "Auto-Switch" is enabled on the PlayerInput
        // component in the Inspector and that control scheme names match binding groups.
        playerInput.onControlsChanged += OnControlsChanged;
        menuManager.Instance.onActionMapSwich += OnChangeInputMap;
        //controls.Gameplay.Move.performed += ctx => tank.moveInput = ctx.ReadValue<Vector2>();
        //controls.Gameplay.Move.canceled += ctx => tank.moveInput = Vector2.zero;
        //controls.Gameplay.Aim.performed += ctx => tank.SetAimInput(ctx.ReadValue<Vector2>());
        //controls.Gameplay.Aim.canceled += ctx => tank.SetAimInput(Vector2.zero);
        //controls.Gameplay.Fire.performed += ctx => tank.fire();
        //controls.Gameplay.menu.performed += ctx => menuManager.instance?.MenuButton();

    }

    private void OnChangeInputMap(string obj)
    {
       playerInput.SwitchCurrentActionMap(obj);
    }

    private void OnControlsChanged(PlayerInput pi)
    {
        // Log the active control scheme for debugging. PlayerInput will already
        // update its current control scheme automatically if Auto-Switch is enabled.
        Debug.Log($"Player {gameObject.name} controls changed. Current scheme: {pi.currentControlScheme}");
        // Optionally you could switch action maps here based on scheme, e.g.:
        // if (pi.currentControlScheme == "Gamepad") playerInput.SwitchCurrentActionMap("Gameplay_alt");
    }


    public void OnMove(InputValue value)
    {
      
        tank.SetMoveInput(value.Get<Vector2>());
    }
    public void OnAim(InputValue value)
    {
        tank.SetAimInput(value.Get<Vector2>());
    }
    public void OnFire(InputValue value)
    {
        tank.fire();
    }
    public void OnEndturn(InputValue value)
    {
        if (isActive)
        {
        GameManager.instance?.NextTurn();
        SetActive(false);
        }
      
    }
    public void OnMenu(InputValue value)
    {
        if(isActive)
        menuManager.Instance?.MenuButton();
    }

    //private void OnEnable()
    //{
    //    //controls.Gameplay.Enable();
    //}

    //private void OnDisable()
    //{
    //    //if (controls != null)
    //    //    controls.Gameplay.Disable();
    //}

    //private void Update()
    //{
    //    if (tank == null)
    //    {
    //        Debug.LogError(name + " has no tank assigned.");
    //        return;
    //    }
        
    //    if (isActive)
    //    {
    //        //tank?.SetMoveInput(Vector2.zero);
    //        // tank?.SetAimInput(Vector2.zero);
    //        //controls.Gameplay.Enable();
    //        return;
    //    }

    //    ////Vector2 moveInput = controls.Gameplay.Move.ReadValue<Vector2>();
    //    //Vector2 aimInput = controls.Gameplay.Aim.ReadValue<Vector2>();

    //    ////tank?.SetMoveInput(moveInput);
    //    //tank?.SetAimInput(aimInput);
    //}

    public void SetActive(bool active)
    {
        isActive = active;

        tank?.SetControlEnabled(active);

        //if (!active)
        //{
        //    //tank?.SetMoveInput(Vector2.zero);
        //    //tank?.SetAimInput(Vector2.zero);
        //    //controls.Gameplay.Disable();
        //}
    }
}
