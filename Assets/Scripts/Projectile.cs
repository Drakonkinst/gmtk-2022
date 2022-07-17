using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 2.0f;
    public float damage = 10.0f;
    public float cannotBreakFor = 1.0f;
    public float lifetime = 5.0f;
    public SoundEffect hitTerrainSound;
    public SoundEffect hitPlayerSound;
    public SoundEffect shootEffect;
    
    private Transform myTransform;
    private float aliveFor = 0.0f;
    
    void Awake() {
        myTransform = transform;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Player player = GameState.instance.GetPlayer();
        myTransform.position = new Vector3(myTransform.position.x, player.GetTransform().position.y, myTransform.position.z);
        myTransform.LookAt(player.GetTransform().position);
        GameState.instance.PlaySound(shootEffect);
    }
    
    void FixedUpdate() {
        myTransform.position += myTransform.forward * speed * Time.deltaTime;
        aliveFor += Time.deltaTime;
        if(aliveFor > lifetime) {
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter(Collision col) {
        if(col.collider.tag == "Terrain" || col.collider.tag == "Dice") {
            GameState.instance.PlaySound(hitTerrainSound);
            Destroy(gameObject);
        } else if(col.collider.tag == "Player") {
            GameState.instance.PlaySound(hitPlayerSound);
            GameState.instance.GetPlayer().Damage(damage);
            Destroy(gameObject);        
        } else if(col.collider.tag == "Enemy" && aliveFor > cannotBreakFor) {
            GameState.instance.PlaySound(hitTerrainSound);
            Destroy(gameObject);
        }
    }
}
