using UnityEngine;

public abstract class BaseToggleComponent : MonoBehaviour, IToggle
{
    private bool state = false;

    public void Activate()
    {
        state = true;
        ActivateComponent();
    }

    public void Deactivate()
    {
        state = false;
        DeactivateComponent();
    }

    protected abstract void ActivateComponent();
    protected abstract void DeactivateComponent();

    public void Switch()
    {
        if (state)
            Deactivate();
        else
            Activate();
    }
}
