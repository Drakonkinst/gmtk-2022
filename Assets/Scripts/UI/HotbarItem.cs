using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarItem : MonoBehaviour
{
    // Set by Hotbar
    private Vector3 focusedSize;
    private Vector3 focusedPos;
    private Quaternion focusedRot;

    private Transform myTransform;
    private Vector3 originalSize;
    private Vector3 originalPos;
    private Quaternion originalRot;
    private Vector3 desiredSize;
    private Vector3 desiredPos;
    private Quaternion desiredRot;
    private bool active = true;
    
    public float speed = 10.0f;
    
    void Awake() {
        myTransform = transform;
        originalSize = myTransform.localScale;
        originalPos = myTransform.localPosition;
        originalRot = myTransform.rotation;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Unfocus();
    }

    // Update is called once per frame
    void Update()
    {
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, desiredRot, Time.deltaTime * speed);
        myTransform.localScale = Vector3.Lerp(myTransform.localScale, desiredSize, Time.deltaTime * speed);
        myTransform.localPosition = Vector3.Lerp(myTransform.localPosition, desiredPos, Time.deltaTime * speed);
    }
    
    public void SetFocusedData(Vector3 focusedSize, Vector3 focusedPos, Quaternion focusedRot) {
        this.focusedSize = focusedSize;
        this.focusedPos = focusedPos;
        this.focusedRot = focusedRot;
    }
    
    public void Focus() {
        desiredSize = focusedSize;
        desiredPos = focusedPos;
        desiredRot = focusedRot;
    }
    
    public void Unfocus() {
        desiredSize = originalSize;
        desiredPos = originalPos;
        desiredRot = originalRot;
    }
    
    public bool IsActive() {
        return active;
    }
}
