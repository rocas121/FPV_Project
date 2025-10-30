using UnityEngine;

public class LampSystemToggle : BaseToggleComponent
{
    [SerializeField] private new GameObject light = null;

    [SerializeField] private new Renderer materialTarget = null;
    [SerializeField] private new Material materialOn = null;
    [SerializeField] private new Material materialOff = null;
    protected override void ActivateComponent()
    {
        Debug.Log("Lamp on");

        materialTarget.material = materialOn;
        light.SetActive(true);

    }
    
    protected override void DeactivateComponent()
    {
        Debug.Log("Lamp off");
        materialTarget.material = materialOff;
        light.SetActive(false);

    }
}
