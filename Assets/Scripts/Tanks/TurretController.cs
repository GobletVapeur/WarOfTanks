using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 120f;

    private Vector2 aimInput;
    private bool controlsEnabled = true;

    private void Update()
    {
        if (!controlsEnabled)
            return;

        RotateTurret();
    }

    private void RotateTurret()
    {
        if (Mathf.Abs(aimInput.x) < 0.01f)
            return;

        float rotation = aimInput.x * turnSpeed * Time.deltaTime;
        transform.Rotate(0f, rotation, 0f);
    }

    public void SetAimInput(Vector2 input)
    {
        aimInput = input;
    }

    public void SetControlEnabled(bool enabled)
    {
        controlsEnabled = enabled;

        if (!enabled)
            aimInput = Vector2.zero;
    }
}