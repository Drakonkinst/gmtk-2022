using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyField : Field
{
    public float range;
    public int numHits = 3;
    
    protected override void Awake() {
        base.Awake();
        yOffset = 1.03f;
    }
    
    protected override void Start() {
        base.Start();
    }
    
    protected override void Update() {
        base.Update();
    }
    
    public void Hit(int damage) {
        numHits -= damage;
        if(numHits <= 0) {
            OnDestroy();
        }
    }
}