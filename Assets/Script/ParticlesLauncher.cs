using UnityEngine;

public class ParticlesLauncher : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem = null;

    public void Start()
    {
    }

    public void Stop()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        particleSystem.Play();
    }
}