using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSize : MonoBehaviour
{
    public float coef;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSize();
    }

    private float DistanceGround()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            return hit.distance;
        }

        return 0;
    }
    private void ChangeSize()
    {
        float distance = DistanceGround();
        float test = (1 - distance / coef);
        if (test < 0.3f)
        {
            applyScale(0.6f, 0.3f, 0.3f);
        }
        else if(test > 0.5)
        {
            applyScale(1f, 0.5f, 0.5f);
        }
        else
        {
            distance = (1- distance / coef);
            applyScale(2* distance, distance, distance);
        }
    }
    private void applyScale(float x,float y,float z)
    {
        this.transform.localScale = new Vector3(x, y, z);
    }
}
