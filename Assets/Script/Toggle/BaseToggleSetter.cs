using UnityEngine;

public abstract class BaseToggleSetter : MonoBehaviour, IToggle
{
    [SerializeField] private BaseToggleComponent toggleComponent = null;

    public void Activate()
    {
        toggleComponent.Activate();
    }

    public void Deactivate()
    {
        toggleComponent.Deactivate();
    }

    public void Switch()
    {
        toggleComponent.Switch();
    }
}
