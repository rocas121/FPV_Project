using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class animController : MonoBehaviour
{

    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            animator.Play("HandPoint", 0, 0f);
        }
    }
}
