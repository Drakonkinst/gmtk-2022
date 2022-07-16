using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public Vector3 focusedSize;
    public Vector3 focusedPos;
    public Vector3 focusedRot;
    
    private HotbarItem[] items;
    private Transform myTransform;
    private int selected = -1;

    void Awake() {
        myTransform = transform;
        items = new HotbarItem[4];
        int index = 0;
        foreach(Transform hotbarItem in myTransform) {
            HotbarItem item = hotbarItem.GetComponent<HotbarItem>();
            item.SetFocusedData(focusedSize, focusedPos, Quaternion.Euler(focusedRot.x, focusedRot.y, focusedRot.z));
            items[index++] = item;
            if(index >= 4) {
                break;
            }
        }
    }
    
    public void Select(int index) {
        Debug.Log("SELECT " + index);
        foreach(HotbarItem item in items) {
            item.Unfocus();
        }
        selected = index;
        if(IsValid(index)) {
            items[index].Focus();
        }
    }
    
    public bool IsValid(int index) {
        return index >= 0 && index < items.Length && items[index].IsActive();
    }
    
    public void SelectNext() {
        for(int i = selected + 1; i < items.Length; ++i) {
            // Really only need the active check here as index should always be good
            // but just in case
            if(IsValid(i)) {
                Select(i);
                return;
            }
        }
        for(int i = 0; i <= selected; ++i) {
            if(IsValid(i)) {
                Select(i);
                return;
            }
        }
        Select(-1);
    }
    
    public void SelectPrev() {
        for(int i = selected - 1; i >= 0; --i) {
            if(IsValid(i)) {
                Select(i);
                return;
            }
        }
        for(int i = items.Length - 1; i >= selected; --i) {
            if(IsValid(i)) {
                Select(i);
                return;
            }
        }
        Select(-1);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
