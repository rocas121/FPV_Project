using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using Unity.VisualScripting;

//Plays the animation when triggered
//Plays sound along with cooldown to avoid spamming
//pick randomly between 2 numbers for multiplier
public class CatTriggerComponent : BaseToggleComponent
{
    [Header("Cat setting")]
    [SerializeField] private AudioClip sound = null;
    [SerializeField] private GameObject idle = null;
    [SerializeField] private GameObject animated = null;
    private float speedMultiplier;

    [SerializeField] private AudioMixer audioMixer;
    private Animator animator;
    private AudioSource audioSource;
    private bool cooldown = true;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = animated.GetComponent<Animator>();
        animated.SetActive(false);
    }
    protected override void ActivateComponent()
    {
        if (cooldown)
        {
            Debug.Log("Playing Cat");
            cooldown = false;
    
            idle.SetActive(false);
            animated.SetActive(true);

            speedMultiplier = Random.value < 0.5f ? 1f : 1.5f; //randoom speed
            if (animator != null)
            {
                animator.speed = speedMultiplier;
            }

            audioSource.pitch = speedMultiplier;
            audioMixer.SetFloat("Pitch", 1f / speedMultiplier);
            audioSource.PlayOneShot(sound);

            StartCoroutine(wait(sound.length / speedMultiplier));
        }



    }

    //faces the camera
    private void LateUpdate()
    {
                transform.forward = Camera.main.transform.forward;
    }

    //return to idle after end of audio
    private IEnumerator wait(float duration)
    {
        yield return new WaitForSeconds(duration);
        idle.SetActive(true);
        animated.SetActive(false);
        cooldown = true;
    }

    //J'avais un singleton pour une activation direct sans toggle mais le nombre max de petit script est 4 
    protected override void DeactivateComponent() {
        ActivateComponent();
    }
}
