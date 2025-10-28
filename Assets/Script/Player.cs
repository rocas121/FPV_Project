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
    }

    public void Player_OnInteract(CallbackContext context)
    {
        Debug.Log("pressed E");
        if (!context.performed)
            return;


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
        input = context.ReadValue<Vector2>();
        input.z = input.y;
        input.y = 0;
    }


    public void Player_OnLook(CallbackContext context)
    {
        rotationInput = context.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        currentRotation.x += -rotationInput.y * rotationSpeed * Time.deltaTime;
        currentRotation.y += rotationInput.x * rotationSpeed * Time.deltaTime;
        currentRotation.x = Mathf.Clamp(currentRotation.x, minMaxYaw.x, minMaxYaw.y);

        root.localRotation = Quaternion.Euler(0, currentRotation.y, 0);
        head.localRotation = Quaternion.Euler(currentRotation.x, 0, 0);
    }

    private void FixedUpdate()
    {
        rigidBody.linearVelocity = root.rotation * (speed * input.normalized);
    }
}
