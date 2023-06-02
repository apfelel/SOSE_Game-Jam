using UnityEngine;
using UnityEngine.UI;

public class SelectOnEnable : MonoBehaviour
{
    void Start()
    {
        GetComponent<Selectable>().Select(); 
    }
}
