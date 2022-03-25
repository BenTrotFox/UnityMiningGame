using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class ObjectNormalToFloor : MonoBehaviour
{
    SpriteRenderer spriteToDistort;
    
    protected void InitilizeSprite()
    {
        //spriteToDistort = this.GetComponent<SpriteRenderer>();
        //this.transform.localScale = new Vector3( 1, Mathf.Sqrt(2), 1);       
        //this.transform.Rotate(-45.0f, 0.0f, 0.0f);

    }
    void Start()
    {      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
