using Unity.XR.OpenVR;
using UnityEngine;

public class ParticleSystemToggleComponent : BaseToggleComponent
{
    [SerializeField] private new ParticleSystem particleSystem = null;

    protected override void ActivateComponent()
    {
        particleSystem.Play();
    }

    protected override void DeactivateComponent()
    {
        particleSystem.Stop();
    }
}
