using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private const float stoppingBuffer = 0.5f;
    
    public string id = "Enemy";
    public float baseSpeed = 5.0f;
    public float turnSpeed = 5.0f;
    public float maxHealth = 100.0f;
    public float stoppingDistance = 2.0f;
    public float contactDamageDistance = 2.2f;
    public float contactDamageInterval = 3.0f;
    public float contactDamage = 10.0f;
    public float cardDropChance = 1.0f;
    public bool canShoot = false;
    public GameObject projectilePrefab;
    public SoundEffect diceHitSound;
    public SoundEffect deathSound;
    public SoundEffect popcornShotSound;
    public SoundEffect hitPlayerSound;
    
    protected NavMeshAgent agent;
    protected Player player;
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
        player = GameState.instance.GetPlayer();
        agent.stoppingDistance = stoppingDistance;
    }
    
    void Update() {
        // Check decoy
        DecoyField nearestDecoy = diceManager.GetNearestDecoy(myTransform.position);
        if(nearestDecoy == null) {
            agent.destination = player.GetTransform().position;
        } else {
            agent.destination = nearestDecoy.transform.position;
            float distSqToDecoy = (myTransform.position - agent.destination).sqrMagnitude;
            if(distSqToDecoy <= contactDamageDistance * contactDamageDistance && Time.time > nextContactDamage) {
                nearestDecoy.Hit(1);
                nextContactDamage = Time.time + contactDamageInterval;
            }
        }
        
        // Check slow fields
        float slowFieldMultiplier = diceManager.GetSlowFieldMultiplier(myTransform.position);
        agent.speed = baseSpeed * slowFieldMultiplier;
        
        // Check damage fields
        float dmgFieldDebuff = diceManager.GetDamageFieldDebuff(myTransform.position);
        if(dmgFieldDebuff > 0.0f) {
            Damage(dmgFieldDebuff * Time.deltaTime);
        }
        
        // Check contact damage
        float distanceSqToPlayer = (myTransform.position - player.GetTransform().position).sqrMagnitude;
        if(distanceSqToPlayer <= contactDamageDistance * contactDamageDistance && Time.time > nextContactDamage) {
            GameState.instance.GetPlayer().Damage(contactDamage);
            GameState.instance.PlaySound(hitPlayerSound);
            nextContactDamage = Time.time + contactDamageInterval;
        }
        if(canShoot && distanceSqToPlayer <= (stoppingDistance + stoppingBuffer) * (stoppingDistance + stoppingBuffer) && Time.time > nextContactDamage) {
            Instantiate(projectilePrefab, myTransform.position, Quaternion.identity, GameState.instance.GetProjectileParent());
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
                GameState.instance.PlaySound(diceHitSound);
                if(player.HasItem("PopcornShot") && dice.numSplits > 0) {
                    // Split
                    Vector3 dicePos = dice.transform.position;
                    FirePopcorn(dicePos, dice.numSplits - 1);
                    FirePopcorn(dicePos, dice.numSplits - 1);
                    GameState.instance.PlaySound(popcornShotSound);
                }
            }
        }
    }
    
    private void FirePopcorn(Vector3 pos, int numSplits) {
        Vector3 dir = new Vector3(Random.value, 0.0f, Random.value).normalized;
        player.GetThrower().Fire(pos, dir, false, numSplits);
    }
    
    private void OnDeath() {
        if(Random.value < cardDropChance) {
            GameState.instance.SpawnItemDrop(myTransform.position, false);
        }
        GameState.instance.GetEnemySpawner().OnDeath(id);
        GameState.instance.PlaySound(deathSound);
        Destroy(gameObject);
    }
}
