using Unity.XR.OpenVR;
using UnityEngine;

public class ParticleSystemToggleComponent : BaseToggleComponent
{
    [SerializeField] private new ParticleSystem particleSystem = null;

    private void Start()
    {
        particleSystem.Stop();
    }
    protected override void ActivateComponent()
    {
        particleSystem.Play();
    }

    protected override void DeactivateComponent()
    {
        particleSystem.Stop();
    }
}
