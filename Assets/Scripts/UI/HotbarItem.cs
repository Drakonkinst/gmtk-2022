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
    private Quaternion flippedRot;
    private Vector3 desiredSize;
    private Vector3 desiredPos;
    private Quaternion desiredRot;
    private bool active = true;
    private bool focused = false;
    private Renderer myRenderer;
    private int index;
    
    public float speed = 10.0f;
    
    void Awake() {
        myTransform = transform;
        originalSize = myTransform.localScale;
        originalPos = myTransform.localPosition;
        originalRot = myTransform.rotation;
        flippedRot = Quaternion.AngleAxis(180.0f, myTransform.forward) * originalRot;
        myRenderer = GetComponent<Renderer>();
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
    
    public void FinishInstant() {
        myTransform.rotation = desiredRot;
        myTransform.localScale = desiredSize;
        myTransform.localPosition = desiredPos;
    }
    
    public void SetFocusedData(int index, Vector3 focusedSize, Vector3 focusedPos, Quaternion focusedRot) {
        this.index = index;
        this.focusedSize = focusedSize;
        this.focusedPos = focusedPos;
        this.focusedRot = focusedRot;
    }
    
    public void SetActive(bool flag) {
        if(focused) {
            return;
        }
        active = flag;
        if(active) {
            desiredRot = originalRot;
        } else {
            desiredRot = flippedRot;
        }
    }
    
    public void SetMaterial(Material matRef) {
        myRenderer.material = matRef;
    }
    
    public void Focus() {
        desiredSize = focusedSize;
        desiredPos = focusedPos;
        desiredRot = focusedRot;
        focused = true;
    }
    
    public void Unfocus() {
        desiredSize = originalSize;
        desiredPos = originalPos;
        desiredRot = originalRot;
        focused = false;
        
        // Reset active due to some wonkiness
        SetActive(active);
    }
    
    public bool IsActive() {
        return active;
    }
}
