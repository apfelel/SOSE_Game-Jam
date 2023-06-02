using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class LevitatingObject : MonoBehaviour
{
    [SerializeField] GameObject targetPickup;
    [SerializeField] GameObject targetParent;
    private List<GameObject> targets = new();
    private List<GameObject> childs = new();
    private float timer;
    [SerializeField] float smoothness;

    bool floating = true;

    Vector3 startPos;
    Quaternion startRot;
    // Start is called before the first frame update
    void Start()
    {
        startPos = targetPickup.transform.position;
        startRot = targetParent.transform.rotation;
        if (targetParent.transform.childCount < 2)
        {
            targetPickup.transform.rotation = targetParent.transform.GetChild(0).transform.rotation;
            targetPickup.transform.position = targetParent.transform.GetChild(0).transform.position;
            return;
        }

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
            gb.transform.rotation = Quaternion.Slerp(gb.transform.rotation, thirdChild.transform.rotation, 0.5f);
            gb.transform.position = secondChild.transform.position - direction * smoothness;
            targets.Add(gb);
            gb.transform.SetParent(targetParent.transform);
            targets.Add(secondChild);
            gb = new GameObject();
            gb.transform.rotation = Quaternion.Slerp(gb.transform.rotation, firstChild.transform.rotation, 0.5f);
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
            secondChild.transform.rotation = Quaternion.Slerp(secondChild.transform.rotation, firstChild.transform.rotation, 0.15f);
            secondChild.transform.rotation = Quaternion.Slerp(secondChild.transform.rotation, thirdChild.transform.rotation, 0.15f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!floating)
        {
            targetPickup.transform.position += (startPos - targetPickup.transform.position).normalized * Time.deltaTime;
            targetPickup.transform.rotation = Quaternion.Slerp(targetPickup.transform.rotation, startRot, Time.deltaTime);
            if (Vector3.Distance(startPos, targetPickup.transform.position) < ((startPos - targetPickup.transform.position).normalized * Time.deltaTime).magnitude)
            {
                if(Quaternion.Angle(startRot, targetPickup.transform.rotation) < 3)
                { 
                targetPickup.transform.position = startPos;
                enabled = false;
                    }
            }
            return;
        }
        if (!targetPickup)
        {
            Destroy(gameObject);
            return;
        }

        if (targets.Count == 0)
            return;
        
        timer += Time.deltaTime;
        targetPickup.transform.position = 
            Vector3.Lerp(
                targets[Mathf.FloorToInt(timer) % targets.Count].transform.position,
                targets[(Mathf.FloorToInt(timer) + 1) % targets.Count].transform.position,
                timer % 1f);

        targetPickup.transform.rotation =
            Quaternion.Slerp(
                targets[Mathf.FloorToInt(timer) % targets.Count].transform.rotation,
                targets[(Mathf.FloorToInt(timer) + 1) % targets.Count].transform.rotation,
                timer % 1f);

    }

    private void OnDrawGizmosSelected()
    {
        foreach(var target in targets)
        {
            Gizmos.DrawLine(target.transform.position, target.transform.forward);
            Gizmos.DrawSphere(target.transform.position, 0.5f);
        }
    }

    public void ResetPosition()
    {
        floating = false;
        GetComponentInChildren<VisualEffect>().SetInt("Particle", 0);
    }
}
