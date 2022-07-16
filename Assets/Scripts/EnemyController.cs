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
    public float stoppingDistance = 2.0f;
    public float contactDamageDistance = 2.2f;
    public float contactDamageInterval = 3.0f;
    public float contactDamage = 10.0f;
    
    protected NavMeshAgent agent;
    protected Transform playerTransform;
    protected Transform myTransform;
    protected DiceManager diceManager;
    protected float health;
    protected bool isDead = false;
    protected float nextContactDamage;

    void Awake() {
        myTransform = transform;
        agent = GetComponent<NavMeshAgent>();
        health = maxHealth;
    }
    
    void Start() {
        diceManager = GameState.instance.GetDiceManager();
        playerTransform = GameState.instance.GetPlayer().GetTransform();
        agent.stoppingDistance = stoppingDistance;
    }
    
    void Update() {
        agent.destination = playerTransform.position;
        
        // Check slow fields
        float slowFieldMultiplier = diceManager.GetSlowFieldMultiplier(myTransform.position);
        agent.speed = baseSpeed * slowFieldMultiplier;
        
        // Check damage fields
        float dmgFieldDebuff = diceManager.GetDamageFieldDebuff(myTransform.position);
        if(dmgFieldDebuff > 0.0f) {
            Damage(dmgFieldDebuff * Time.deltaTime);
        }
        
        // Check contact damage
        float distanceSqToPlayer = (myTransform.position - playerTransform.position).sqrMagnitude;
        if(distanceSqToPlayer <= contactDamageDistance * contactDamageDistance && Time.time > nextContactDamage) {
            GameState.instance.GetPlayer().Damage(contactDamage);
            nextContactDamage = Time.time + contactDamageInterval;
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
