using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{ 
    private TankController tank;

    //private TankControls controls;
    private bool isActive = true;
    private PlayerInput playerInput;
    private bool invertedControls = false;
    private bool invertedaim = false;

    private bool invertLeftRequested = false;
    private bool invertRightRequested = false;
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
        // Subscribe to left/right joystick inversion toggles
        menuManager.Instance.onInvertLeftJoystickToggle += OnInvertLeftJoystickToggle;
        menuManager.Instance.onInvertRightJoystickToggle += OnInvertRightJoystickToggle;
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
        // When control scheme changes, apply any pending joystick inversion setting
        ApplyInvertIfJoystick(pi.currentControlScheme);
        // Optionally you could switch action maps here based on scheme, e.g.:
        // if (pi.currentControlScheme == "Gamepad") playerInput.SwitchCurrentActionMap("Gameplay_alt");
    }


    public void OnMove(InputValue value)
    {

        Vector2 mv = value.Get<Vector2>();
        if (invertedControls)
        {
            mv = -mv;
        }
        tank.SetMoveInput(mv);
    }
    public void OnAim(InputValue value)
    {
        Vector2 aim = value.Get<Vector2>();
        if (invertedaim)
        {
            // Invert Y axis for aim when requested
            aim.y = -aim.y;
        }
        tank.SetAimInput(aim);
    }
    public void OnFire(InputValue value)
    {
        tank.fire();
    }

    private void OnInvertLeftJoystickToggle(bool value)
    {
        invertLeftRequested = value;
        if (playerInput != null)
            ApplyInvertIfJoystick(playerInput.currentControlScheme);
    }

    private void OnInvertRightJoystickToggle(bool value)
    {
        invertRightRequested = value;
        if (playerInput != null)
            ApplyInvertIfJoystick(playerInput.currentControlScheme);
    }

    private void ApplyInvertIfJoystick(string scheme)
    {
        if (string.IsNullOrEmpty(scheme))
            return;

        string s = scheme.ToLowerInvariant();
        bool isJoystick = s.Contains("gamepad") || s.Contains("joystick") || s.Contains("xbox") || s.Contains("ps4") || s.Contains("ps5");

        if (isJoystick)
        {
            invertedControls = invertLeftRequested; // left stick controls (move)
            invertedaim = invertRightRequested;    // right stick controls (aim)
        }
        else
        {
            invertedControls = false;
            invertedaim = false;
        }
    }

    private void OnDestroy()
    {
        if (menuManager.Instance != null)
        {
            menuManager.Instance.onInvertLeftJoystickToggle -= OnInvertLeftJoystickToggle;
            menuManager.Instance.onInvertRightJoystickToggle -= OnInvertRightJoystickToggle;
        }
        if (playerInput != null)
            playerInput.onControlsChanged -= OnControlsChanged;
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
