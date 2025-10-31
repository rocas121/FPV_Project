using UnityEngine;

public abstract class BaseTriggerComponent : MonoBehaviour, ITrigger
{
    public void Activate()
    {
        ActivateComponent();
    }

    protected abstract void ActivateComponent();
}
