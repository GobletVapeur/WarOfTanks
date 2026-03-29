namespace Core
{
    using UnityEngine;
    using UnityEngine.UI;

    public class WorldHUDController : MonoBehaviour
    {
        [SerializeField] private Image healthBarFill;

        private TankController targetTank;
        private Camera cam;

        public void Initialize(TankController tank)
        {
            targetTank = tank;
            cam = Camera.main;
        }

        private void Update()
        {
            if (targetTank == null)
                return;

            // Position écran
            Vector3 screenPos = cam.WorldToScreenPoint(targetTank.HudAnchor.position);

            if (screenPos.z < 0)
            {
                gameObject.SetActive(false);
                return;
            }

            transform.position = screenPos;

            // Update vie
            float percent = targetTank.Stats.CurrentHealth / targetTank.Stats.MaxHealth;
            
            RectTransform rt = healthBarFill.rectTransform;
            rt.pivot = new Vector2(0f, 0.5f); // origine à gauche

            healthBarFill.transform.localScale = new Vector3(percent, 1, 1);
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}