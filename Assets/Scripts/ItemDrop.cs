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
    public float rotationPerSecond = 60.0f;
    public float velocity = 20.0f;
    public float velocityDecayPerSecond = 20.0f;
    public SoundEffect itemPickupSound;
    
    private Transform myTransform;
    private Renderer myRenderer;
    private Player player;
    private float pickupTime = 0.0f;
    private Vector3 dir;
    
    void Awake() {
        myTransform = transform;
        myRenderer = GetComponent<Renderer>();
        dir = new Vector3(Random.value, 0.0f, Random.value);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameState.instance.GetPlayer();
        myTransform.position = new Vector3(myTransform.position.x, itemHeight, myTransform.position.z);
    }
    
    void Update() {
        myTransform.Rotate(new Vector3(0.0f, rotationPerSecond * Time.deltaTime, 0.0f));
        if(Time.time > pickupTime) {
            float distSqToPlayer = (myTransform.position - player.GetTransform().position).sqrMagnitude;
            if(distSqToPlayer < pickupDistance * pickupDistance) {
                if(player.HasOpenSlot()) {
                    GameState.instance.PlaySound(itemPickupSound);
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
    
    void FixedUpdate() {
        if(velocity > 0) {
            myTransform.position += dir * velocity * Time.deltaTime;
            velocity -= velocityDecayPerSecond * Time.deltaTime;
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
