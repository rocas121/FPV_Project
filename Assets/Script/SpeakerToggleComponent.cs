using UnityEngine;

public class SpeakerToggleComponent : BaseToggleComponent
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Renderer childRenderer;
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;

    protected override void ActivateComponent()
    {
        audioSource.Play();
        particles.Play();
        childRenderer.sharedMaterial = onMaterial;
    }

    protected override void DeactivateComponent()
    {
        audioSource.Stop();
        particles.Stop();
        childRenderer.sharedMaterial = offMaterial;
    }
}
