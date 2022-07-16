using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private const float AngleRotation = 90.0f;
    
    public float activationTime = 1.0f;
    public float disableTime = 0.1f;
    private Rigidbody rb;
    private float currRestTime = 0.0f;
    private Transform myTransform;
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
        myTransform = transform;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.velocity.sqrMagnitude < Mathf.Epsilon) {
            currRestTime += Time.deltaTime;
            if(currRestTime > activationTime) {
                float xRotation = Mathf.Round(myTransform.rotation.eulerAngles.x / AngleRotation) * AngleRotation;
                float zRotation = Mathf.Round(myTransform.rotation.eulerAngles.z / AngleRotation) * AngleRotation;
                int face = GetFaceFromRotation(xRotation, zRotation);
                Debug.Log(face);
                Destroy(gameObject);
            }
        } else {
            currRestTime = 0.0f;
        }
    }
    
    public bool ShouldDamage() {
        return currRestTime < disableTime;
    }
    
    private int GetFaceFromRotation(float x, float z) {
        if(x == 270 && z == 0) {
            return 1;
        }
        if(x == 0 && z == 0) {
            return 2;
        }
        if(x == 0 && z == 270) {
            return 3;
        }
        if(x == 0 && z == 90) {
            return 4;
        }
        if(x == 0 && z == 180) {
            return 5;
        }
        if(x == 90 && z == 0) {
            return 6;
        }
        return -999;
    }
}
