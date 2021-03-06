using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hotbar : MonoBehaviour
{
    public Vector3 focusedSize;
    public Vector3 focusedPos;
    public Vector3 focusedRot;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public GameObject itemInfo;
    public SoundEffect swapCardSound;
    
    private HotbarItem[] items;
    private string[] inventory;
    private Transform myTransform;
    private int selected = -1;

    void Awake() {
        myTransform = transform;
        items = new HotbarItem[4];
        int index = 0;
        foreach(Transform hotbarItem in myTransform) {
            HotbarItem item = hotbarItem.GetComponent<HotbarItem>();
            item.SetFocusedData(index, focusedSize, focusedPos, Quaternion.Euler(focusedRot.x, focusedRot.y, focusedRot.z));
            items[index++] = item;
            if(index >= 4) {
                break;
            }
        }
    }
    
    public void SetContents(string[] inventory) {
        if(inventory.Length != items.Length) {
            Debug.LogWarning("Inventory and Hotbar length should match (" + inventory.Length + " vs " + items.Length + ")");
        }
        this.inventory = inventory;
        for(int i = 0; i < items.Length; ++i) {
            if(inventory[i] == null || inventory[i].Length <= 0) {
                items[i].SetActive(false);
                continue;
            }
            
            string id = inventory[i];
            ItemEntry entry = GameState.instance.GetItemInfo(id);
            if(entry.id == "") {
                // Set slot inactive
                items[i].SetActive(false);
                continue;
            }
            
            items[i].SetMaterial(entry.card);
            items[i].SetActive(true);
        }
    }
    
    public void FinishInstant() {
        foreach(HotbarItem item in items) {
            item.FinishInstant();
        }
    }
    
    public void Select(int index) {
        //Debug.Log("SELECT " + index);
        // Reset
        itemInfo.SetActive(false);
        itemName.text = "";
        itemDescription.text = "";
        
        foreach(HotbarItem item in items) {
            item.Unfocus();
        }
        if(selected != index) {
            GameState.instance.PlaySound(swapCardSound);
        }
        selected = index;
        if(IsValid(index)) {
            items[index].Focus();
            itemInfo.SetActive(true);
            ItemEntry entry = GameState.instance.GetItemInfo(inventory[index]);
            itemName.text = entry.name;
            if(entry.stackable) {
                itemDescription.text = "(Stackable)";
            } else {
                itemDescription.text = "(Unstackable)";
            }
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
    
    public void UnselectAll() {
        Select(-1);
    }
    
    public int GetSelectedIndex() {
        return selected;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        UnselectAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
