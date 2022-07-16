using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public string id = "Enemy";
    public float baseSpeed = 5.0f;
    public float turnSpeed = 5.0f;
    public float maxHealth = 100.0f;
    
    protected NavMeshAgent agent;
    protected Transform playerTransform;
    protected Transform myTransform;
    protected DiceManager diceManager;
    protected float health;
    protected bool isDead = false;

    void Awake() {
        myTransform = transform;
        agent = GetComponent<NavMeshAgent>();
        health = maxHealth;
    }
    
    void Start() {
        diceManager = GameState.instance.GetDiceManager();
        playerTransform = GameState.instance.GetPlayer().transform;
    }
    
    void Update() {
        agent.destination = playerTransform.position;
        float slowFieldMultiplier = diceManager.GetSlowFieldMultiplier(myTransform.position);
        agent.speed = baseSpeed * slowFieldMultiplier;
        
        float dmgFieldDebuff = diceManager.GetDamageFieldDebuff(myTransform.position);
        if(dmgFieldDebuff > 0.0f) {
            Damage(dmgFieldDebuff * Time.deltaTime);
        }
        
        if(isDead) {
            OnDeath();
        }
    }
    
    public void Damage(float damage) {
        health -= damage;
        if(health <= Mathf.Epsilon) {
            health = 0.0f;
            isDead = true;
        }
    }
    
    void LookToward(Vector3 destination, float distance) {
        Vector3 turnTowardNavSteeringTarget = agent.steeringTarget;
        Vector3 direction = (turnTowardNavSteeringTarget - myTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, lookRotation, Time.deltaTime / turnSpeed);
    }
    
    void OnCollisionEnter(Collision collision) {
        if(collision.collider.tag == "Dice") {
            Dice dice = collision.gameObject.GetComponent<Dice>();
            if(dice.ShouldDamage(gameObject.GetInstanceID())) {
                Damage(60.0f);
            }
        }
    }
    
    private void OnDeath() {
        GameState.instance.GetEnemySpawner().OnDeath(id);
        Destroy(gameObject);
    }
}
