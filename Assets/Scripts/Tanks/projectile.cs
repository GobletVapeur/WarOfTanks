using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        TankController tank = collision.collider.GetComponentInParent<TankController>();

        if (tank != null)
        {
            tank.Stats.TakeDamage(30f);
            
            GameManager.instance.CheckEndGame();
        }

        Destroy(gameObject);
    }
}