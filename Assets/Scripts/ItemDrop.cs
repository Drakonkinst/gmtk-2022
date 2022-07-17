using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    private const float itemHeight = 0.17f;
    public string id;
    public float pickupDistance = 1.5f;
    public float dropPickupDelay = 1.0f;
    public float lootPickupDelay = 0.2f;
    public float lifetime = 60.0f;
    
    private Transform myTransform;
    private Renderer myRenderer;
    private Player player;
    private float pickupTime = 0.0f;
    
    void Awake() {
        myTransform = transform;
        myRenderer = GetComponent<Renderer>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameState.instance.GetPlayer();
        myTransform.position = new Vector3(myTransform.position.x, itemHeight, myTransform.position.z);
    }
    
    void Update() {
        if(Time.time > pickupTime) {
            float distSqToPlayer = (myTransform.position - player.GetTransform().position).sqrMagnitude;
            if(distSqToPlayer < pickupDistance * pickupDistance) {
                if(player.HasOpenSlot()) {
                    player.AddItem(id);
                    Destroy(gameObject);
                }
            }
        }
        lifetime -= Time.deltaTime;
        if(lifetime <= 0.0f) {
            Destroy(gameObject);
        }
    }
    
    public void SetItem(string id) {
        this.id = id;
        ItemEntry info = GameState.instance.GetItemInfo(id);
        myRenderer.material = info.card;
    }
    
    public void AfterDrop() {
        pickupTime = Time.time + dropPickupDelay;
    }
    
    public void AfterLoot() {
        pickupTime = Time.time + lootPickupDelay;
    }
}
