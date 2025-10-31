using UnityEngine;

public class TriggerToggleSetter : BaseToggleSetter
{
    private void OnTriggerEnter(Collider other)
    {
        Activate();
    }

    private void OnTriggerExit(Collider other)
    {
        Deactivate();
    }
}
