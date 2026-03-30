using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TankStats))]
public class TankController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private TurretController turret;
    [Header("Weapons")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 25f;

    private Rigidbody rb;
    private TankStats stats;
    private Transform hudAnchor;
    public Vector2 moveInput;
    private bool controlsEnabled = true;

    public TankStats Stats => stats;
    public bool ControlsEnabled => controlsEnabled;
    public Transform HudAnchor => hudAnchor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<TankStats>();
        hudAnchor = transform.Find("HudAnchor");
        
        if (turret == null)
        {
            turret = GetComponentInChildren<TurretController>();
        }
    }

    private void FixedUpdate()
    {
        if (!controlsEnabled || !stats.HasStamina)
        {
            turret?.SetAimInput(Vector2.zero);
            return;
        }
       
        HandleTurretStamina();
        MoveTank();
        RotateTank();
    }
    public void fire()
    { 
        if (!controlsEnabled)
            return;

       if(!stats.ConsumeFire())
            return;

        // Determine spawn position and rotation: prefer explicit firePoint, then turret barrel, then tank forward
        Transform barrel = turret != null ? turret.Barrel : null;
        Vector3 spawnPos = firePoint != null ? firePoint.position : (barrel != null ? barrel.position : transform.position + transform.forward * 1.5f);
        Quaternion spawnRot = firePoint != null ? firePoint.rotation : (barrel != null ? barrel.rotation : transform.rotation);

        if (projectilePrefab == null)
        {
            Debug.LogError("TankController: projectilePrefab is not assigned on " + name + ". Cannot fire.");
            return;
        }

        GameObject proj = Instantiate(projectilePrefab, spawnPos, spawnRot);

        Rigidbody projRb = proj.GetComponent<Rigidbody>();
        if (projRb == null)
        {
            // Try to add a Rigidbody if missing so the projectile can move, but warn the developer.
            Debug.LogWarning("Projectile prefab " + projectilePrefab.name + " has no Rigidbody. Adding one at runtime.");
            projRb = proj.AddComponent<Rigidbody>();
        }

        // Use the correct Rigidbody property 'velocity' to set linear speed
        projRb.linearVelocity = spawnRot * Vector3.forward * projectileSpeed;

        // Destroy projectile after 5 seconds to avoid lingering objects
        Destroy(proj, 5f);
    }

    private void MoveTank()
    {
        float signedRequestedDistance = moveInput.y * moveSpeed * Time.fixedDeltaTime;
        float requestedDistance = Mathf.Abs(signedRequestedDistance);

        if (requestedDistance <= 0f || !stats.HasStamina)
        {
             if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            return;
        }

        float allowedDistance = stats.GetMaxMovableDistance(requestedDistance);
        if (allowedDistance <= 0f)
            return;

        float direction = signedRequestedDistance > 0f ? 1f : -1f;

        Vector3 movement = transform.forward * (allowedDistance * direction);

        rb.MovePosition(rb.position + movement);

        stats.ConsumeStamina(allowedDistance);
        
    }

    private void RotateTank()
    {
        float input = moveInput.x;

        if (Mathf.Abs(input) < 0.01f)
            return;

        float rotation = input * turnSpeed * Time.fixedDeltaTime;

        float cost = Mathf.Abs(rotation) * stats.RotationStaminaCostPerDegree;

        if (cost > stats.CurrentStamina)
            return;

        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turn);

        stats.ConsumeStamina(cost);
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
        if (!controlsEnabled || turret == null)
            return;

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
    
    private void HandleTurretStamina()
    {
        if (turret == null)
            return;

        if (!stats.HasStamina)
        {
            turret.SetAimInput(Vector2.zero);
            return;
        }

        Vector2 input = turret.AimInput;
        float totalCost = 0f;

        // --- YAW (horizontal) ---
        if (Mathf.Abs(input.x) >= 0.01f)
        {
            float yawRotation = input.x * turret.TurnSpeed * Time.fixedDeltaTime;
            totalCost += Mathf.Abs(yawRotation) * stats.TurretStaminaCostPerDegree;
        }

        // --- PITCH (vertical, avec clamp correct) ---
        float correctedY = turret.InvertPitch ? input.y : -input.y;

        if (Mathf.Abs(correctedY) >= 0.01f)
        {
            float current = turret.Barrel.localEulerAngles.x;
            if (current > 180f) current -= 360f;

            float delta = correctedY * turret.PitchSpeed * Time.fixedDeltaTime;
            float target = current + delta;

            bool blocked =
                target > turret.MaxPitch ||
                target < turret.MinPitch;

            if (!blocked)
            {
                totalCost += Mathf.Abs(delta) * stats.TurretStaminaCostPerDegree;
            }
        }

        if (totalCost <= 0f)
            return;

        if (totalCost > stats.CurrentStamina)
        {
            turret.SetAimInput(Vector2.zero);
            return;
        }

        stats.ConsumeStamina(totalCost);
    }
    
}
