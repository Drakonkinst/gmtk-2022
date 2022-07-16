using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowField : Field
{   
    public float speedMultiplier = 0.3f;
    
    protected override void Start() {
        base.Start();
    }
    
    protected override void Update() {
        base.Update();
    }
}