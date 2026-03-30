using UnityEngine;

public class CameraFollowTurret : MonoBehaviour
{
    [SerializeField] private float height = 10f;
    [SerializeField] private float distance = 15f;

    private Transform target;
    private TurretController turret;

    public void SetTarget(TankController tank)
    {
        target = tank.transform;
        turret = tank.GetComponentInChildren<TurretController>();
    }

    private void LateUpdate()
    {
        if (target == null || turret == null) return;

        // direction de la tourelle
        Vector3 forward = turret.transform.forward;
        forward.y = 0;
        forward.Normalize();

        // position derrière la tourelle
        Vector3 pos = target.position 
                      - forward * distance 
                      + Vector3.up * height;

        transform.position = pos;

        // regarde vers l'avant du tank
        transform.LookAt(target.position + forward * 5f);
    }
}