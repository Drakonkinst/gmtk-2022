using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public string id = "Enemy";
    public float turnSpeed = 5.0f;
    
    private NavMeshAgent agent;
    private Transform playerTransform;
    private Transform myTransform;

    void Awake() {
        myTransform = transform;
        agent = GetComponent<NavMeshAgent>();
    }
    
    void Start() {
        playerTransform = GameState.instance.GetPlayer().transform;
    }
    
    void Update() {
        agent.destination = playerTransform.position;
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
            if(dice.ShouldDamage()) {
                OnDeath();
            }
        }
    }
    
    void OnDeath() {
        GameState.instance.GetEnemySpawner().OnDeath(id);
        Destroy(gameObject);
    }
}
