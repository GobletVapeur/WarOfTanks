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

        float healthPercent = currentTank.Stats.CurrentHealth / currentTank.Stats.MaxHealth;
        float staminaPercent = currentTank.Stats.CurrentStamina / currentTank.Stats.MaxStamina;

        healthBarFill.transform.localScale = new Vector3(healthPercent, 1, 1);
        staminaBarFill.transform.localScale = new Vector3(staminaPercent, 1, 1);
    }
}