using UnityEngine;


//Set Active light source and change material when toggled on and off
public class LampSystemToggle : BaseToggleComponent
{
    [SerializeField] private new GameObject light = null;

    [Header("Material setting")]
    [SerializeField] private Renderer materialTarget = null;
    [SerializeField] private Material materialOn = null;
    [SerializeField] private Material materialOff = null;
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
