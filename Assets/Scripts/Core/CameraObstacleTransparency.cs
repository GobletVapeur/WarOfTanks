using UnityEngine;
using System.Collections.Generic;

public class CameraObstacleTransparency : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float transparency = 0.3f;

    private List<Renderer> currentTransparent = new List<Renderer>();
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

    private void LateUpdate()
    {
        if (target == null) return;

        // Reset anciens murs
        foreach (var r in currentTransparent)
        {
            if (r != null && originalColors.ContainsKey(r))
            {
                r.material.color = originalColors[r];
            }
        }

        currentTransparent.Clear();

        // Raycast
        Vector3 dir = target.position - transform.position;
        float distance = dir.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir.normalized, distance, obstacleLayer);

        foreach (var hit in hits)
        {
            Renderer r = hit.collider.GetComponent<Renderer>();
            if (r == null) continue;

            if (!originalColors.ContainsKey(r))
            {
                originalColors[r] = r.material.color;
            }

            Color c = r.material.color;
            c.a = transparency;
            r.material.color = c;

            currentTransparent.Add(r);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}