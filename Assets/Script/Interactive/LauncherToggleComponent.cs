using UnityEngine;
using System.Collections;

public class AirstrikeDesignator : BaseToggleComponent
{
    [Header("Refs")]
    [SerializeField] private Player player;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform playerHead;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Transform buildingsRoot;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private ParticleSystem markFX;
    [SerializeField] private ParticleSystem impactFX;
    [SerializeField] private Canvas targetingCanvas;

    [Header("Config")]
    [SerializeField] private float hoverHeight = 100f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float markDelay = 5f;
    [SerializeField] private float groundY = 0f;
    [SerializeField] private float dropHeight = 160f;
    [SerializeField] private float bombSpeed = 100f;

    [Header("Debug")]
    [SerializeField] private Vector3 lastMarkedPoint;

    Vector3 startPos;
    Quaternion startRot, startCamRot;
    bool savedGravity;
    float savedSpeed;
    bool isDesignating = false;

    protected override void ActivateComponent() 
    { 
        if (!isDesignating)
            StartCoroutine(Designate()); 
    }
    
    protected override void DeactivateComponent() {}

    void Update()
    {
        if (isDesignating && player != null)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            
            if (h != 0 || v != 0)
            {
                Vector3 move = (playerRoot.right * h + playerRoot.forward * v).normalized;
                playerRoot.position += move * moveSpeed * Time.deltaTime;
            }
        }
    }

    IEnumerator Designate()
    {
        isDesignating = true;

        // Sauvegarde de la position/rotation du joueur
        startPos = playerRoot.position;
        startRot = playerRoot.rotation;
        startCamRot = playerHead.localRotation;
        savedGravity = playerRb.useGravity;
        savedSpeed = GetSpeed();

        // Désactive les contrôles normaux
        player.canMove = false;
        playerRb.useGravity = false;
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        yield return null;

        // TP au dessus de la zone
        Vector3 above = buildingsRoot 
            ? new Vector3(buildingsRoot.position.x, hoverHeight, buildingsRoot.position.z)
            : new Vector3(startPos.x, hoverHeight, startPos.z);

        playerRb.position = above;
        playerRoot.position = above;
        playerRoot.rotation = Quaternion.identity;
        playerHead.localRotation = Quaternion.Euler(90f, 0f, 0f);

        if (targetingCanvas != null)
            targetingCanvas.gameObject.SetActive(true);

        yield return null;

        // Phase de visée
        yield return new WaitForSeconds(markDelay);

        if (targetingCanvas != null)
            targetingCanvas.gameObject.SetActive(false);

        // Point d'impact sous la caméra
        Vector3 camPos = playerHead.position;
        lastMarkedPoint = new Vector3(camPos.x, groundY, camPos.z);

        if (markFX)
        {
            markFX.transform.SetPositionAndRotation(
                lastMarkedPoint + Vector3.up * 0.1f,
                Quaternion.Euler(-90f, 0f, 0f)
            );
            markFX.Play();
        }

        // Restaure le joueur
        playerRb.position = startPos;
        playerRoot.SetPositionAndRotation(startPos, startRot);
        playerHead.localRotation = startCamRot;
        playerRb.useGravity = savedGravity;
        SetSpeed(savedSpeed);
        player.canMove = true;

        isDesignating = false;

        StartCoroutine(DropBomb(lastMarkedPoint));

        Deactivate();
    }

    IEnumerator DropBomb(Vector3 target)
    {
        GameObject bomb = Instantiate(bombPrefab, target + Vector3.up * dropHeight, Quaternion.Euler(90f, 0f, 0f));
        var rb = bomb.GetComponent<Rigidbody>();
        
        if (rb == null)
            rb = bomb.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Chute de la bombe
        while (bomb != null && bomb.transform.position.y > groundY + 0.5f)
        {
            rb.MovePosition(bomb.transform.position + Vector3.down * bombSpeed * Time.deltaTime);
            yield return null;
        }

        if (bomb == null) yield break;

        // Explosion
        if (impactFX)
        {
            impactFX.transform.SetPositionAndRotation(
                target + Vector3.up * 0.5f,
                Quaternion.Euler(-90f, 0f, 0f)
            );
            impactFX.Play();
        }

        DestroyAllBuildings();
        Destroy(bomb);
    }

    void DestroyAllBuildings()
    {
        if (buildingsRoot == null) return;

        int childCount = buildingsRoot.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(buildingsRoot.GetChild(i).gameObject);
        }
    }

    float GetSpeed()
    {
        var f = typeof(Player).GetField("speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return f != null ? (float)f.GetValue(player) : 5f;
    }

    void SetSpeed(float v)
    {
        var f = typeof(Player).GetField("speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (f != null) f.SetValue(player, v);
    }
}