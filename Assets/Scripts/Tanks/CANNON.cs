using UnityEngine;
using UnityEngine.InputSystem;

public class CANNON : MonoBehaviour
{
    [Header("Projectile")]
    [Tooltip("Prefab to instantiate when firing.")]
    public GameObject projectile;

    [Header("Launch Settings")]
    [Tooltip("Local offset applied after the spawn point is computed.")]
    public Vector3 muzzleOffset = Vector3.zero;
    [Tooltip("Force applied to the projectile's rigidbody along the spawn forward.")]
    public float launchForce = 700f;
    public float projectileLifetime = 10f;

    [Header("Aiming")]
    [Tooltip("Degrees change per key press for yaw/pitch.")]
    public float rotationStep = 15f;
    [Tooltip("Minimum pitch (degrees).")]
    public float minPitch = -80f;
    [Tooltip("Maximum pitch (degrees).")]
    public float maxPitch = 80f;
    [Tooltip("If true, spawn the projectile from the top face of the cannon instead of in front.")]
    public bool shootFromTop = true;

    // internal stored angles (signed)
    private float pitch;
    private float yaw;
    // muzzle local pose computed once in Start (face chosen once)
    private Vector3 muzzleLocalPos;
    private Quaternion muzzleLocalRot;

    void Start()
    {
        // Initialize stored angles from current local rotation to avoid reading Euler angles repeatedly
        Vector3 e = transform.localEulerAngles;
        pitch = NormalizeAngle(e.x);
        yaw = NormalizeAngle(e.y);
        // apply explicitly to ensure consistent representation
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);

        // compute muzzle local pose once
        ComputeMuzzleLocalPose();
    }

    private void ComputeMuzzleLocalPose()
    {
        // Determine face once: top face if shootFromTop, otherwise front face
        Vector3 dir = shootFromTop ? transform.up : transform.forward;
        // inspect colliders on this GameObject only
        Collider[] cols = GetComponents<Collider>();
        Vector3 bestPoint = transform.position;
        if (cols != null && cols.Length > 0)
        {
            float bestProj = float.NegativeInfinity;
            foreach (var cc in cols)
            {
                if (cc == null) continue;
                Bounds b = cc.bounds;
                float extentProj = Mathf.Abs(Vector3.Dot(b.extents, dir));
                Vector3 facePoint = b.center + dir * extentProj;
                float proj = Vector3.Dot(dir, facePoint);
                if (proj > bestProj)
                {
                    bestProj = proj;
                    bestPoint = facePoint;
                }
            }
        }

        float padding = 0.01f;
        Vector3 worldMuzzlePos = bestPoint + dir * padding + transform.TransformVector(muzzleOffset);
        Quaternion worldMuzzleRot = Quaternion.LookRotation(dir, (shootFromTop ? transform.forward : transform.up));

        // store local pose so it follows transform at runtime
        muzzleLocalPos = transform.InverseTransformPoint(worldMuzzlePos);
        muzzleLocalRot = Quaternion.Inverse(transform.rotation) * worldMuzzleRot;
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        // Pitch (up/down) -- operate on local X and clamp to avoid flipping
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            AdjustPitch(-rotationStep);
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            AdjustPitch(rotationStep);

        // Yaw (left/right) -- rotate around local Y
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            AdjustYaw(-rotationStep);
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            AdjustYaw(rotationStep);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            Fire();
    }

    private void AdjustPitch(float delta)
    {
        pitch = Mathf.Clamp(pitch + delta, minPitch, maxPitch);
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void AdjustYaw(float delta)
    {
        yaw = NormalizeAngle(yaw + delta);
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    // Normalize angle to range (-180,180]
    private float NormalizeAngle(float angle)
    {
        angle = Mathf.Repeat(angle + 180f, 360f) - 180f;
        return angle;
    }

    public void Fire()
    {
        if (projectile == null)
            return;

        // use precomputed muzzle local pose
        Vector3 spawnPos = transform.TransformPoint(muzzleLocalPos);
        Quaternion spawnRot = transform.rotation * muzzleLocalRot;

        GameObject go = Instantiate(projectile, spawnPos, spawnRot);
        Destroy(go, projectileLifetime);

        // Prevent collisions between projectile and this cannon (only same-gameobject colliders)
        Collider[] projCols = go.GetComponentsInChildren<Collider>();
        Collider[] cannonCols = GetComponents<Collider>();
        foreach (var pc in projCols)
            foreach (var cc in cannonCols)
                if (pc != null && cc != null)
                    Physics.IgnoreCollision(pc, cc);

        Rigidbody rb = go.GetComponent<Rigidbody>() ?? go.GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            if (rb.isKinematic) rb.isKinematic = false;
            rb.AddForce(go.transform.forward * launchForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("CANNON: instantiated projectile has no Rigidbody.");
        }
    }
}
