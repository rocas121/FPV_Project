using UnityEngine;

public class TrailToggleComponent : BaseToggleComponent
{
    [SerializeField] private TrailRenderer trailRenderer = null;

    protected override void ActivateComponent()
    {
        trailRenderer.emitting = true;
    }

    protected override void DeactivateComponent()
    {
        trailRenderer.emitting = false;
    }
}
