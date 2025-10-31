using UnityEngine;

public class GameObjectToggleComponent : BaseToggleComponent
{
    [SerializeField] private GameObject target = null;

    private void Reset()
    {
        target = gameObject;
    }

    protected override void ActivateComponent()
    {
        target.SetActive(true);
    }

    protected override void DeactivateComponent()
    {
        target.SetActive(false);
    }
}
