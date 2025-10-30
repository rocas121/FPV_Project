using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    [Header("Player movement control")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Rigidbody rigidBody = null;
    public bool canMove = true;

    [Header("Interaction distance")]
    [SerializeField] private Vector2 minMaxYaw = new(-90f, 90f);
    [SerializeField] private int rayDistance = 5;
    [SerializeField] private LayerMask interactionMask = default;
    [SerializeField] private Transform root = null;
    [SerializeField] private Transform head = null;
    public InteractionToggleSetter forcedInteraction = null; // AJOUT : Pour forcer l'interaction pendant le sommeil


    [Header("Shoot Mechanic")]
    [SerializeField] private GameObject bulletHolePrefab = null;
    [SerializeField] private GameObject vfx = null;
    [SerializeField] private AudioClip sound = null;
    private AudioSource audioSource;

    private Vector3 input = Vector3.zero;
    private Vector2 rotationInput;
    private Vector2 currentRotation;




    private void Reset()
    {
        rigidBody = GetComponent<Rigidbody>();

    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        audioSource = GetComponent<AudioSource>();
    }

    public void Player_OnInteract(CallbackContext context)
    {

        if (!context.performed)
            return;
        Debug.Log("pressed E");
        // AJOUT : Si on a une interaction forc�e (ex: pendant le sommeil), l'utiliser directement
        if (forcedInteraction != null)
        {
            Debug.Log("Using forced interaction");
            forcedInteraction.Interact();
            return;
        }

        Ray ray = new Ray(head.position, head.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, interactionMask))
        {
            if (hit.collider.TryGetComponent(out InteractionToggleSetter interactionToggleSetter))
                interactionToggleSetter.Interact();
        }
    }

    public void Player_OnMove(CallbackContext context)
    {
        if (!canMove) return;

        input = context.ReadValue<Vector2>();
        input.z = input.y;
        input.y = 0;
    }

    public void Player_OnLook(CallbackContext context)
    {
        if (!canMove) return;

        rotationInput = context.ReadValue<Vector2>();
    }

    public void Player_Shoot(CallbackContext context)
    {

        if (!context.performed)
            return;

        Ray ray = new Ray(head.position, head.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, interactionMask))
        {
            Debug.Log("Target hit");
            if (bulletHolePrefab != null)
            {
                //Sprite position
                Vector3 spawnPos = hit.point + hit.normal * 0.01f;
                Quaternion rotation = Quaternion.LookRotation(hit.normal);
                float randomAngle = Random.Range(0f, 360f);
                rotation *= Quaternion.Euler(0, 0, randomAngle);

                GameObject hole = Instantiate(bulletHolePrefab, spawnPos, rotation);
                GameObject explosion = Instantiate(vfx, spawnPos, rotation);

                audioSource.PlayOneShot(sound);
                hole.transform.SetParent(hit.collider.transform);
                explosion.transform.SetParent(hit.collider.transform);

                Destroy(explosion, 2f);
                Destroy(hole, 5f);
            }
        }
    }

    private void LateUpdate()
    {
        if (!canMove) return;

        currentRotation.x += -rotationInput.y * rotationSpeed * Time.deltaTime;
        currentRotation.y += rotationInput.x * rotationSpeed * Time.deltaTime;
        currentRotation.x = Mathf.Clamp(currentRotation.x, minMaxYaw.x, minMaxYaw.y);

        root.localRotation = Quaternion.Euler(0, currentRotation.y, 0);
        head.localRotation = Quaternion.Euler(currentRotation.x, 0, 0);
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            rigidBody.linearVelocity = Vector3.zero;
            return;
        }

        rigidBody.linearVelocity = root.rotation * (speed * input.normalized);
    }
}