using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceIcon : MonoBehaviour
{
    private Quaternion desiredRotation;
    private Vector3 desiredScale;
    private Transform myTransform;
    public float turnSpeed = 10.0f;
    public float scaleSpeed = 10.0f;
    public Vector3 baseScale = new Vector3(27.0f, 27.0f, 27.0f);
    
    private int currentFace = -1;
    
    void Awake() {
        myTransform = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, desiredRotation, Time.deltaTime * turnSpeed);
        myTransform.localScale = Vector3.Lerp(myTransform.localScale, desiredScale, Time.deltaTime * scaleSpeed);
    }
    
    public void SetFaceInstant(int num) {
        SetFace(num);
        myTransform.rotation = desiredRotation;
    }
    
    public void SetFace(int num) {
        if(currentFace == num) {
            return;
        }
        
        if(num == 0) {
            desiredScale = Vector3.zero;
        } else {
            desiredScale = baseScale;
        }
        
        if(num == 1) {
            desiredRotation = Quaternion.Euler(0, 180, 270);
        } else if(num == 2) {
            desiredRotation = Quaternion.Euler(0, 90, 270);
        } else if(num == 3) {
            desiredRotation = Quaternion.Euler(0, 90, 180);
        } else if(num == 4) {
            desiredRotation = Quaternion.Euler(0, 90, 0);
        } else if(num == 5) {
            desiredRotation = Quaternion.Euler(0, 90, 90);
        } else if(num == 6) {
            desiredRotation = Quaternion.Euler(0, 0, 90);
        }
        currentFace = num;
    }
}
