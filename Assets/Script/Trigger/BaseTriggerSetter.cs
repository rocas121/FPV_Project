using UnityEngine;

public abstract class BaseTriggerSetter : MonoBehaviour, ITrigger
{
    [SerializeField] private BaseTriggerComponent triggerComponent = null;
    public void Activate()
    {
        triggerComponent.Activate();
    }

}
