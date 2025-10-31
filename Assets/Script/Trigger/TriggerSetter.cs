using UnityEngine;

public class TriggerSetter : BaseTriggerSetter
{

    private void OnTriggerEnter(Collider other)
    {
        Activate();
    }
    


}
