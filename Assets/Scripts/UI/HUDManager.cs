using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image staminaBarFill;

    private TankController currentTank;

    public void SetActiveTank(TankController tank)
    {
        currentTank = tank;
    }

    private void Update()
    {
        if (currentTank == null)
            return;

        float healthPercent = (float)currentTank.Stats.CurrentHealth / currentTank.Stats.MaxHealth;
        float staminaPercent = (float)currentTank.Stats.CurrentStamina / currentTank.Stats.MaxStamina;

        healthPercent = Mathf.Clamp01(healthPercent);
        staminaPercent = Mathf.Clamp01(staminaPercent);

        healthBarFill.rectTransform.localScale = new Vector3(healthPercent, 1f, 1f);
        staminaBarFill.rectTransform.localScale = new Vector3(staminaPercent, 1f, 1f);
    }
}