using System;
using UnityEngine;

[DisallowMultipleComponent]
public class TankStats : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string tankId = "Tank";

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private float startingStamina = 10f;
    [SerializeField] private float staminaRegenPerTurn = 5f;
    [SerializeField] private float movementStaminaCostPerUnit = 1f;

    public event Action<TankStats> StaminaChanged;

    public string TankId => tankId;
    public float MaxStamina => maxStamina;
    public float CurrentStamina { get; private set; }
    public float StaminaRegenPerTurn => staminaRegenPerTurn;
    public float MovementStaminaCostPerUnit => movementStaminaCostPerUnit;
    public bool HasStamina => CurrentStamina > 0f;

    private void Awake()
    {
        maxStamina = Mathf.Max(0f, maxStamina);
        startingStamina = Mathf.Clamp(startingStamina, 0f, maxStamina);
        staminaRegenPerTurn = Mathf.Max(0f, staminaRegenPerTurn);
        movementStaminaCostPerUnit = Mathf.Max(0f, movementStaminaCostPerUnit);

        CurrentStamina = startingStamina;
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

    public bool ConsumeMovement(float travelledDistance)
    {
        float sanitizedDistance = Mathf.Max(0f, travelledDistance);

        if (sanitizedDistance <= 0f)
        {
            return true;
        }

        if (movementStaminaCostPerUnit <= 0f)
        {
            return true;
        }

        float staminaCost = sanitizedDistance * movementStaminaCostPerUnit;
        if (staminaCost > CurrentStamina)
        {
            return false;
        }

        SetCurrentStamina(CurrentStamina - staminaCost);
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
}
