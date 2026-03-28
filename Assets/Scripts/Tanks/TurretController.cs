using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private Transform barrel;
    [SerializeField] private float pitchSpeed = 60f;
    [SerializeField] private float minPitch = -10f;
    [SerializeField] private float maxPitch = 30f;
    [SerializeField] private bool invertPitch = false;

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
        // Horizontal rotation of the turret
        if (Mathf.Abs(aimInput.x) >= 0.01f)
        {
            float rotation = aimInput.x * turnSpeed * Time.deltaTime;
            transform.Rotate(0f, rotation, 0f);
        }

        // Vertical rotation (pitch) of the barrel relative to the turret
        if (Mathf.Abs(aimInput.y) >= 0.01f)
        {
            float inputY = invertPitch ? aimInput.y : -aimInput.y;
            float delta = inputY * pitchSpeed * Time.deltaTime;

            // Convert current local X rotation to signed angle (-180..180)
            float current = barrel.localEulerAngles.x;
            if (current > 180f) current -= 360f;

            float target = Mathf.Clamp(current + delta, minPitch, maxPitch);

            Vector3 localEuler = barrel.localEulerAngles;
            localEuler.x = target;
            barrel.localEulerAngles = localEuler;
        }
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

    // Expose barrel transform for external use (e.g. spawning projectiles)
    public Transform Barrel => barrel;
}