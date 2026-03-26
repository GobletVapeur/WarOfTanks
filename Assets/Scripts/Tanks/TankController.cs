using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TankStats))]
public class TankController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private TurretController turret;

    private Rigidbody rb;
    private TankStats stats;

    private Vector2 moveInput;
    private bool controlsEnabled = true;

    public TankStats Stats => stats;
    public bool ControlsEnabled => controlsEnabled;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<TankStats>();
        
        if (turret == null)
        {
            turret = GetComponentInChildren<TurretController>();
        }
    }

    private void FixedUpdate()
    {
        if (!controlsEnabled)
        {
            return;
        }

        MoveTank();
        RotateTank();
    }

    private void MoveTank()
    {
        float signedRequestedDistance = moveInput.y * moveSpeed * Time.fixedDeltaTime;
        float requestedDistance = Mathf.Abs(signedRequestedDistance);

        if (requestedDistance <= 0f || !stats.HasStamina)
            return;

        float allowedDistance = stats.GetMaxMovableDistance(requestedDistance);
        if (allowedDistance <= 0f)
            return;

        float direction = signedRequestedDistance > 0f ? 1f : -1f;

        Vector3 movement = transform.forward * (allowedDistance * direction);

        rb.MovePosition(rb.position + movement);

        stats.ConsumeMovement(allowedDistance);
    }

    private void RotateTank()
    {
        float rotation = moveInput.x * turnSpeed * Time.fixedDeltaTime;
        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);

        rb.MoveRotation(rb.rotation * turn);
    }

    public void SetControlEnabled(bool enabled)
    {
        controlsEnabled = enabled;

        if (!enabled)
        {
            moveInput = Vector2.zero;
            turret?.SetAimInput(Vector2.zero);
        }
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }
    
    public void SetAimInput(Vector2 input)
    {
        if (turret == null)
        {
            Debug.LogError(name + " has no TurretController!");
            return;
        }

        turret.SetAimInput(input);
    }

    public void BeginTurn()
    {
        stats.RegenerateForTurnStart();
        SetControlEnabled(true);
    }

    public void EndTurn()
    {
        SetControlEnabled(false);
    }
}
