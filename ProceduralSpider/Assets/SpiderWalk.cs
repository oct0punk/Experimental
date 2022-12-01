using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWalk : MonoBehaviour
{
    public float speed;
    public Transform target;
    public SpiderLeg[] legs;

    private void Awake()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            legs[i].spider = this;
            legs[i].pos = legs[i].target.position;
        }
    }

    // Update is called once per frame  
    void Update()
    {
        // Input move
        transform.position += Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) *
            new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"))
            * speed * Time.deltaTime;

        if (target)
            transform.LookAt(target);

        Vector3 pos = transform.position;
        pos.y = altitude();
        transform.position = Vector3.Lerp(transform.position, pos, speed);

        for (int i = 0; i < legs.Length; i++)
        {
            legs[i].Update();
        }
    }

    float altitude() // Average of all legs
    {
        float average = 0;
        for (int i = 0; i < legs.Length; i++)
        {
            average += legs[i].pos.y;
        }
        average /= legs.Length;
        return average;
    }


    private void OnDrawGizmos()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            Gizmos.DrawCube(transform.position + legs[i].rayBegin, new Vector3(.3f, .3f, .3f));
        }
    }
}

[System.Serializable]
public struct SpiderLeg
{
    [HideInInspector] public Vector3 pos;
    [HideInInspector] public SpiderWalk spider;
    public Transform target;
    public Vector3 rayBegin;
    private Vector3 ray;

    public void Update()
    {
        target.position = Vector3.Lerp(target.position, pos, 20 * Time.deltaTime);
        ray = spider.transform.TransformPoint(rayBegin);
        ray += Vector3.up * 5;

        Debug.DrawRay(ray, Vector3.down * 20, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, Vector3.down, out hit, 10))
        {
            Debug.DrawRay(
                target.position + Vector3.up * .1f,
                hit.point - target.position,
                Color.green);


            if (Vector3.Distance(target.position, hit.point) > 1.2f)
            {
                float transformZ = spider.transform.InverseTransformDirection(hit.point - target.position).z;
                float signZ = Mathf.Sign(transformZ);
                float transformX = spider.transform.InverseTransformDirection(hit.point - target.position).x;
                float signX = Mathf.Sign(transformX);

                if (Mathf.Abs(transformZ) > Mathf.Abs(transformX))
                    pos = ReplaceTargetForward(signZ);
                //pos = ReplaceTargetOpposite(hit.point - target.position);
                else
                    pos = ReplaceTargetSide(signX);
                //pos = ReplaceTargetOpposite(hit.point - target.position);

            }
        }
    }


    private Vector3 ReplaceTargetForward(float sign)
    {
        Vector3 coord = spider.transform.TransformPoint(rayBegin);
        coord += spider.transform.forward * sign;

        RaycastHit hit;
        if (Physics.Raycast(coord, Vector3.down, out hit, 5))
            return hit.point;
        else
            return coord - Vector3.down * 3;
    }

    private Vector3 ReplaceTargetSide(float sign)
    {
        Vector3 coord = spider.transform.TransformPoint(rayBegin);
        coord += spider.transform.right * sign;

        RaycastHit hit;
        if (Physics.Raycast(coord, Vector3.down, out hit, 5))
            return hit.point;
        else
            return coord - Vector3.down * 3;
    }

    private Vector3 ReplaceTargetOpposite(Vector3 direction)
    {
        Vector3 coord = spider.transform.TransformPoint(rayBegin);
        coord += direction * .8f;

        RaycastHit hit;
        if (Physics.Raycast(coord, Vector3.down, out hit, 5))
            return hit.point;
        else
            return coord - Vector3.down * 3;
    }

}
