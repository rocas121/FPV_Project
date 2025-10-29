using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Rigidbody rigidBody = null;
    [SerializeField] private Vector2 minMaxYaw = new(-90f, 90f);
    [SerializeField] private int rayDistance = 5;
    [SerializeField] private LayerMask interactionMask = default;
    [SerializeField] private Transform root = null;
    [SerializeField] private Transform head = null;

    [SerializeField] private GameObject bulletHolePrefab = null;

    private Vector3 input = Vector3.zero;
    private Vector2 rotationInput;
    private Vector2 currentRotation;

    public bool canMove = true;
    public InteractionToggleSetter forcedInteraction = null; // AJOUT : Pour forcer l'interaction pendant le sommeil

    private void Reset()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Player_OnInteract(CallbackContext context)
    {

        if (!context.performed)
            return;
        Debug.Log("pressed E");
        // AJOUT : Si on a une interaction forcée (ex: pendant le sommeil), l'utiliser directement
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

                GameObject hole = Instantiate(bulletHolePrefab, spawnPos, rotation);
                hole.transform.SetParent(hit.collider.transform);

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