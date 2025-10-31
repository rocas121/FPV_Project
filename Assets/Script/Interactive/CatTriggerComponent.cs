using UnityEngine;
using UnityEngine.Audio;

public class CatTriggerComponent : BaseTriggerComponent
{
    [Header("Cat setting")]
    [SerializeField] private AudioClip sound = null;
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    protected override void ActivateComponent()
    {
        audioSource.PlayOneShot(sound);
        Debug.Log("Trigger");

    }


}
