using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    public bool Cleaning;

    private Animator anim;
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetFloat("Speed", rb.velocity.magnitude);
    }
    public void PlayCleaning()
    {
        anim.Play("Cleaning");
        Cleaning = true;

        StartCoroutine(TestDelay());
    }
    IEnumerator TestDelay()
    {
        yield return new WaitForSeconds(2);
        EndCleaning();
    }
    public void EndCleaning()
    {
        Cleaning = false;
    }
}
