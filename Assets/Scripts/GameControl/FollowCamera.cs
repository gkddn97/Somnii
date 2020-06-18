﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 옛날 카메라 무빙, 지금은 안씀
public class FollowCamera:MonoBehaviour
{

    public float smoothTimeX, smoothTimeY;
    public Vector2 velocity;
    public GameObject player;
    public Vector2 minPos, maxPos;
    public bool boundCheck;

    //private GameObject bound;
    //private Vector3 minBound;
    //private Vector3 maxBound;
    //private float halfWidth;
    //private float halfHeight;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //bound = GameObject.Find("Bound00");
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        float posX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
        float posY = Mathf.SmoothDamp(transform.position.y, player.transform.position.y, ref velocity.y, smoothTimeY);

        transform.position = new Vector3(posX, posY, transform.position.z);

        if(boundCheck) {
            transform.position = new Vector3( Mathf.Clamp(transform.position.x, minPos.x, maxPos.x), 
                Mathf.Clamp(transform.position.y, minPos.y, maxPos.y), 
                Mathf.Clamp(transform.position.z, transform.position.z, transform.position.z) );
        }
    }

}
