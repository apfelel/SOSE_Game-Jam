using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitatingObject : MonoBehaviour
{
    [SerializeField] GameObject targetPickup;
    [SerializeField] GameObject targetParent;
    private List<GameObject> targets = new();

    [SerializeField] private float duration;

    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in targetParent.transform)
        {
            targets.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetPickup)
        {
            Destroy(gameObject);
            return;
        }
        timer += Time.deltaTime;

        targetPickup.transform.position = 
            Vector3.Lerp(
                targets[Mathf.FloorToInt(timer) % targets.Count].transform.position,
                targets[(Mathf.FloorToInt(timer) + 1) % targets.Count].transform.position,
                timer % 1f);
    }
}
