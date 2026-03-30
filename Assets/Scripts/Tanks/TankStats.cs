using System;
using UnityEngine;

[DisallowMultipleComponent]
public class TankStats : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string tankId = "Tank";

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 20f;
    [SerializeField] private float startingStamina = 20f;
    [SerializeField] private float staminaRegenPerTurn = 10f;
    [SerializeField] private float movementStaminaCostPerUnit = 1f;
    [SerializeField] private float turretStaminaCostPerDegree = 0.01f;
    [SerializeField] private float rotationStaminaCostPerDegree = 0.05f;
    [SerializeField] private float maxHealth = 100f;
    
    public event Action<TankStats> StaminaChanged;
    public float TurretStaminaCostPerDegree => turretStaminaCostPerDegree;
    public string TankId => tankId;
    public float MaxStamina => maxStamina;
    public float CurrentStamina { get; private set; }
    public bool HasStamina => CurrentStamina > 0f;
    public float StaminaRegenPerTurn => staminaRegenPerTurn;
    public float MovementStaminaCostPerUnit => movementStaminaCostPerUnit;
    public float RotationStaminaCostPerDegree => rotationStaminaCostPerDegree;
    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0f;
    

    private void Awake()
    {
        maxStamina = Mathf.Max(0f, maxStamina);
        startingStamina = Mathf.Clamp(startingStamina, 0f, maxStamina);
        staminaRegenPerTurn = Mathf.Max(0f, staminaRegenPerTurn);
        movementStaminaCostPerUnit = Mathf.Max(0f, movementStaminaCostPerUnit);

        CurrentStamina = startingStamina;
        CurrentHealth = maxHealth;
    }

    public void RegenerateForTurnStart()
    {
        SetCurrentStamina(CurrentStamina + staminaRegenPerTurn);
    }

    public float GetMaxMovableDistance(float requestedDistance)
    {
        float sanitizedDistance = Mathf.Max(0f, requestedDistance);

        if (sanitizedDistance <= 0f)
        {
            return 0f;
        }

        if (movementStaminaCostPerUnit <= 0f)
        {
            return sanitizedDistance;
        }

        float maxDistance = CurrentStamina / movementStaminaCostPerUnit;
        return Mathf.Min(sanitizedDistance, maxDistance);
    }

    public bool ConsumeStamina(float amount)
    {
        float sanitizedAmount = Mathf.Max(0f, amount);

        if (sanitizedAmount <= 0f)
            return true;

        if (sanitizedAmount > CurrentStamina)
            return false;

        SetCurrentStamina(CurrentStamina - sanitizedAmount);
        return true;
    }

    private void SetCurrentStamina(float value)
    {
        float clampedValue = Mathf.Clamp(value, 0f, maxStamina);
        if (Mathf.Approximately(clampedValue, CurrentStamina))
        {
            return;
        }

        CurrentStamina = clampedValue;
        StaminaChanged?.Invoke(this);
    }
    
    public void TakeDamage(float amount)
    {
        if (amount <= 0f || !IsAlive)
            return;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);
    }

    internal bool ConsumeFire()
    {   
        if (CurrentStamina  <= 5f)
        {
            return false;
        }
        CurrentStamina -=5f;   
        return true;
    }
}
