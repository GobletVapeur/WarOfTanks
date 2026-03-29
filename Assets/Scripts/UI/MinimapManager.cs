using UnityEngine;


public class MinimapManager : MonoBehaviour
{
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private float cameraHeight = 50f;

    private Transform _target;

    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 pos = _target.position;
        pos.y = cameraHeight;

        minimapCamera.transform.position = pos;
        minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}