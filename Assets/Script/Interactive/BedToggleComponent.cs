using UnityEngine;
using System.Collections;

public class BedToggleComponent : BaseToggleComponent
{
    [Header("Refs")]
    [SerializeField] private Player playerController;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform bedAnchor;
    [SerializeField] private RectTransform topLid;
    [SerializeField] private RectTransform bottomLid;
    [SerializeField] private Collider bedTrigger;
    [SerializeField] private InteractionToggleSetter interactionToggleSetter;

    [Header("Audio")]
    [SerializeField] private AudioSource snoreLoopSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip yawnClip;

    [Header("Timing")]
    [SerializeField] private float preDelay = 0.3f;
    [SerializeField] private float rotateUpDuration = 0.6f;
    [SerializeField] private float eyelidDuration = 0.4f;
    [SerializeField] private float lookUpAngle = 75f;

    private Vector3 startPlayerPos;
    private Quaternion startPlayerRot;
    private Quaternion startCamLocalRot;
    private bool isSleeping = false;

    private void Start()
    {
        if (interactionToggleSetter == null)
            interactionToggleSetter = GetComponent<InteractionToggleSetter>();
    }

    protected override void ActivateComponent()
    {
        if (isSleeping) return;
        StopAllCoroutines();
        StartCoroutine(SleepSequence());
    }

    protected override void DeactivateComponent()
    {
        if (!isSleeping) return;
        StopAllCoroutines();
        StartCoroutine(WakeSequence());
    }

    IEnumerator SleepSequence()
    {
        isSleeping = true;

        startPlayerPos = playerRoot.position;
        startPlayerRot = playerRoot.rotation;
        startCamLocalRot = cameraTransform.localRotation;

        Vector3 bedFwd = bedAnchor.forward;
        bedFwd.y = 0f;
        bedFwd.Normalize();
        Quaternion bedYaw = Quaternion.LookRotation(bedFwd, Vector3.up);

        if (bedTrigger) bedTrigger.enabled = false;

        if (playerController)
        {
            playerController.canMove = false;
            playerController.forcedInteraction = interactionToggleSetter;
        }

        yield return null;

        // TP dans le lit
        playerRoot.position = bedAnchor.position;
        playerRoot.rotation = bedYaw;
        cameraTransform.localRotation = Quaternion.identity;

        yield return new WaitForSeconds(0.2f);

        if (bedTrigger) bedTrigger.enabled = true;

        if (sfxSource && yawnClip) sfxSource.PlayOneShot(yawnClip);

        yield return new WaitForSeconds(preDelay);

        yield return RotatePitchAndLids(0f, lookUpAngle, rotateUpDuration, eyelidDuration, true);

        if (snoreLoopSource) snoreLoopSource.Play();
    }

    IEnumerator WakeSequence()
    {
        if (snoreLoopSource) snoreLoopSource.Stop();
        if (sfxSource && yawnClip) sfxSource.PlayOneShot(yawnClip);

        yield return RotatePitchAndLids(
            cameraTransform.localEulerAngles.x,
            startCamLocalRot.eulerAngles.x,
            rotateUpDuration,
            eyelidDuration,
            false
        );

        if (bedTrigger) bedTrigger.enabled = false;

        playerRoot.position = startPlayerPos;
        playerRoot.rotation = startPlayerRot;
        cameraTransform.localRotation = startCamLocalRot;

        yield return null;

        if (playerController)
        {
            playerController.canMove = true;
            playerController.forcedInteraction = null;
        }

        if (bedTrigger) bedTrigger.enabled = true;

        isSleeping = false;
    }

    IEnumerator RotatePitchAndLids(float fromPitch, float toPitch, float rotDur, float lidsDur, bool closing)
    {
        rotDur = Mathf.Max(0.0001f, rotDur);
        lidsDur = Mathf.Max(0.0001f, lidsDur);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / rotDur;
            float tt = Mathf.Clamp01(t);

            float pitch = Mathf.LerpAngle(fromPitch, toPitch, tt);
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

            float k = Mathf.Clamp01(tt * rotDur / lidsDur);
            float offset = Mathf.Lerp(500f, 0f, closing ? k : 1f - k);
            SetLids(offset);

            yield return null;
        }

        cameraTransform.localRotation = Quaternion.Euler(toPitch, 0f, 0f);
        SetLids(closing ? 0f : 500f);
    }

    void SetLids(float offset)
    {
        if (topLid != null)
            topLid.anchoredPosition = new Vector2(0, offset);

        if (bottomLid != null)
            bottomLid.anchoredPosition = new Vector2(0, -offset);
    }
}