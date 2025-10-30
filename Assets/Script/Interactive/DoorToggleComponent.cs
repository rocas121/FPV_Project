using UnityEngine;

public class DoorToggleComponent : BaseToggleComponent
{
    [SerializeField] private Transform door;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Vector3 openRotation = new(0, 90, 0);
    [SerializeField] private float speed = 3f;

    private Vector3 closedRotation;
    private Quaternion targetRotation;

    private void Awake()
    {
        closedRotation = door.localEulerAngles;
        targetRotation = Quaternion.Euler(closedRotation);
    }

    private void Update()
    {
        door.localRotation = Quaternion.Lerp(door.localRotation, targetRotation, Time.deltaTime * speed);
    }

    protected override void ActivateComponent()
    {
        targetRotation = Quaternion.Euler(openRotation);
        audioSource.Play();
    }

    protected override void DeactivateComponent()
    {
        targetRotation = Quaternion.Euler(closedRotation);
        audioSource.Play();
    }
}
