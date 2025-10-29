using UnityEngine;
using System.Collections;

public class Launcher : BaseToggleComponent
{
    [Header("Refs")]
    [SerializeField] private Player playerController;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private GameObject missileCanvas;
    [SerializeField] private ParticleSystem impactFX;

    [Header("Config")]
    [SerializeField] private Vector3 offset = new(-20f, 100f, 0f);
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float probeDistance = 2f;
    [SerializeField] private LayerMask groundMask = ~0;

    private Vector3 startPos;
    private Quaternion startRot;

    protected override void ActivateComponent() => StartCoroutine(LaunchSequence());
    protected override void DeactivateComponent() { }

    private IEnumerator LaunchSequence()
    {
        startPos = playerRoot.position;
        startRot = playerRoot.rotation;

        if (missileCanvas) missileCanvas.SetActive(true);

        playerRoot.position = startPos + offset;
        playerRoot.rotation = startRot;
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
        playerRb.useGravity = false;

        float speed = 0f;
        RaycastHit hit;

        while (true)
        {
            // vitesse qui augmente
            speed += acceleration * Time.deltaTime;

            // direction = regard caméra (mise à jour en continu)
            Vector3 dir = playerCamera.forward.normalized;

            // appliquer la vélocité
            playerRb.linearVelocity = dir * speed;

            // détection d'impact devant
            if (Physics.Raycast(playerRoot.position, dir, out hit, probeDistance + speed * Time.deltaTime, groundMask))
                break;

            yield return null;
        }

        // impact
        if (impactFX)
        {
            impactFX.transform.position = hit.point;
            impactFX.transform.rotation = Quaternion.LookRotation(hit.normal);
            impactFX.Play();
        }

        yield return new WaitForSeconds(0.1f);

        // reset
        playerRoot.position = startPos;
        playerRoot.rotation = startRot;
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
        playerRb.useGravity = true;

        if (missileCanvas) missileCanvas.SetActive(false);

        Deactivate();
    }
}
