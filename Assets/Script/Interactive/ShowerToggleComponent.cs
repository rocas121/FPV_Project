using UnityEngine;

//Turn particle and change button material when toggled on and off
public class ShowerToggleComponent : BaseToggleComponent
{
    [SerializeField] private new ParticleSystem particleSystem = null;

    [Header("Material setting")]
    [SerializeField] private Renderer materialTarget = null;
    [SerializeField] private Material materialOn = null;
    [SerializeField] private Material materialOff = null;
    private void Start()
    {
        materialTarget.material = materialOff;
        particleSystem.Stop();
    }
    protected override void ActivateComponent()
    {
        materialTarget.material = materialOn;
        particleSystem.Play();
    }

    protected override void DeactivateComponent()
    {
        materialTarget.material = materialOff;
        particleSystem.Stop();
    }
}
