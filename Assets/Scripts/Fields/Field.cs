using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Field : MonoBehaviour
{   
    private const float imageMultiplier = 0.2f;
    private const float height = -0.13f;
    
    public float maxDuration = 5;
    public float radius = 10.0f;
    protected float yOffset = 0.0f;
    protected float timeRemaining;
    protected DiceManager diceManager;
    protected Vector3 center;
    
    protected virtual void Awake() {

    }
    
    protected virtual void Start() {
        timeRemaining = maxDuration;
        diceManager = GameState.instance.GetDiceManager();
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
        center = transform.position;
        transform.localScale = new Vector3(radius * imageMultiplier, 1, radius * imageMultiplier);
    }
    
    protected virtual void Update() {
        timeRemaining -= Time.deltaTime;
        if(timeRemaining <= 0) {
            OnDestroy();
        }
    }
    
    protected void OnDestroy() {
        diceManager.OnFieldExpire(this);
        Destroy(gameObject);
    }
    
    public bool IsInField(Vector3 pos) {
        float deltaX = Mathf.Abs(center.x - pos.x);
        float deltaZ = Mathf.Abs(center.z - pos.z);
        return deltaX * deltaX + deltaZ * deltaZ < radius * radius;
    }
}