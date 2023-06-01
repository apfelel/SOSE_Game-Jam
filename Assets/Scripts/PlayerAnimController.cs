using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetFloat("velocity", rb.velocity.magnitude);
    }
    public void PlayCleaning()
    {
    }
}
