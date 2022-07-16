using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    
    private Transform myTransform;
    
    void Awake() {
        myTransform = transform;
    }
    
    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(myTransform.position, targetPos, smoothSpeed);
        myTransform.position = smoothedPos;
    }
}
