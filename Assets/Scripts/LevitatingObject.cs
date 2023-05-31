using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class LevitatingObject : MonoBehaviour
{
    [SerializeField] GameObject targetPickup;
    [SerializeField] GameObject targetParent;
    private List<GameObject> targets = new();
    private List<GameObject> childs = new();
    [SerializeField] private float duration;
    private float timer;
    [SerializeField] float smoothness;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in targetParent.transform)
        {
            childs.Add(child.gameObject);
        }
        for(int i = 0; i < childs.Count; i++)
        {
            GameObject firstChild;
            if(i == 0)
                firstChild = childs[childs.Count - 1];
            else
                firstChild = childs[i - 1];


            GameObject secondChild = childs[i];
            GameObject thirdChild = childs[(i + 1) % childs.Count];

            var direction = (thirdChild.transform.position - firstChild.transform.position).normalized;
            var gb = new GameObject();
            gb.transform.position = secondChild.transform.position - direction * smoothness;
            targets.Add(gb);
            gb.transform.SetParent(targetParent.transform);
            targets.Add(secondChild);
            gb = new GameObject();
            gb.transform.position = secondChild.transform.position + direction * smoothness;
            targets.Add(gb);
            gb.transform.SetParent(targetParent.transform);
        }

        for (int i = 0; i < targets.Count; i++)
        {
            GameObject firstChild;
            if (i == 0)
                firstChild = targets[childs.Count - 1];
            else
                firstChild = targets[i - 1];
            GameObject secondChild = targets[i];
            GameObject thirdChild = targets[(i + 1) % childs.Count];

            secondChild.transform.position = (secondChild.transform.position) * 0.7f + (firstChild.transform.position * 0.15f) + (thirdChild.transform.position * 0.15f);
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
                targets[(Mathf.FloorToInt(timer)) % targets.Count].transform.position,
                targets[(Mathf.FloorToInt(timer) + 1) % targets.Count].transform.position,
                timer % 1f);

    }

    private void OnDrawGizmosSelected()
    {
        foreach(var target in targets)
        {
            Gizmos.DrawSphere(target.transform.position, 0.5f);
        }
    }
}
