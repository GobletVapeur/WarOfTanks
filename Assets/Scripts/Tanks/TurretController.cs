using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 120f;

    private Vector2 aimInput;

    private void Update()
    {
        RotateTurret();
    }

    private void RotateTurret()
    {
        float rotation = aimInput.x * turnSpeed * Time.deltaTime;
        transform.Rotate(0f, rotation, 0f);
    }

    public void SetAimInput(Vector2 input)
    {
        aimInput = input;
    }
}
