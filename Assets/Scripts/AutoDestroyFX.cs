using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AutoDestroyFX : MonoBehaviour
{
    private VisualEffect fx;
    private bool t = false;
    // Start is called before the first frame update
    void Start()
    {
        fx = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fx.aliveParticleCount == 0)
        {
            if(t == true)
                Destroy(gameObject);
        }
        else
        {
            t = true;
        }
    }
}
