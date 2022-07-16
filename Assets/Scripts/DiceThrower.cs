using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody))]
public class DiceThrower : MonoBehaviour
{
    public GameObject dicePrefab;
    public Transform diceParent;
    public float spawnOffsetForward = 5;
    public float spawnOffsetUp = 0;
    public float throwSpeedForward = 500.0f;
    public float throwSpeedUp = 100.0f;
    public float momentumMultiplier = 40.0f;
    public float tumble = 50.0f;
    
    private Transform myTransform;
    private PlayerController movement;
    private Rigidbody rb;
    
    void Awake() {
        myTransform = transform;
        movement = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnFire() {
        Vector3 spawnPos = myTransform.position + myTransform.forward * spawnOffsetForward + new Vector3(0.0f, spawnOffsetUp, 0.0f);
        GameObject dice = Instantiate(dicePrefab, spawnPos, myTransform.rotation, diceParent);
        Rigidbody diceRb = dice.GetComponent<Rigidbody>();
        
        float forwardSpeed = throwSpeedForward;
        if(movement.IsMoving()) {
            forwardSpeed += movement.GetMaxSpeed() * momentumMultiplier;
        }
        Vector3 velocity = dice.transform.forward * forwardSpeed + dice.transform.up * throwSpeedUp;
        diceRb.angularVelocity = Random.insideUnitSphere * tumble;
        diceRb.rotation = Quaternion.Euler(new Vector3(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f)));
        diceRb.AddForce(velocity);
    }
}
