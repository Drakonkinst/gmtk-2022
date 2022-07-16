using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private const float AngleRotation = 90.0f;
    private const float doublingDistance = 8.0f;
    
    public float activationTime = 1.0f;
    public float disableTime = 0.1f;
    
    private Rigidbody rb;
    private float currRestTime = 0.0f;
    private Transform myTransform;
    private HashSet<int> wasHit = new HashSet<int>();
    private int face = -1;
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
        myTransform = transform;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.velocity.sqrMagnitude < Mathf.Epsilon) {
            currRestTime += Time.deltaTime;
            if(face < 0) {
                float xRotation = Mathf.Round(myTransform.rotation.eulerAngles.x / AngleRotation) * AngleRotation;
                float zRotation = Mathf.Round(myTransform.rotation.eulerAngles.z / AngleRotation) * AngleRotation;
                face = GetFaceFromRotation(xRotation, zRotation);
                Debug.Log(face);
            }
            if(currRestTime > activationTime) {
                bool doubled = CheckDoubled();
                DoDiceEffects(face, doubled);
                OnDestroy();
            }
        } else {
            currRestTime = 0.0f;
            face = -1;
        }
    }
    
    private void OnDestroy() {
        Destroy(gameObject);
    }
    
    private bool CheckDoubled() {
        Transform diceParent = GameState.instance.GetDiceParent();
        foreach(Transform diceTransform in diceParent) {
            if(diceTransform == myTransform) {
                continue;
            }
            Dice dice = diceTransform.gameObject.GetComponent<Dice>();
            if(dice.face == face) {
                float distSq = (diceTransform.position - myTransform.position).sqrMagnitude;
                if(distSq < doublingDistance * doublingDistance) {
                    dice.OnDestroy();
                    return true;
                }
            }
        }
        return false;
    }
    
    private void DoDiceEffects(int face, bool doubled) {
        if(face == 2) {
            if(doubled) {
                GameState.instance.GetDiceManager().SpawnBigSlowField(myTransform.position);
            } else {
                GameState.instance.GetDiceManager().SpawnSmallSlowField(myTransform.position);
            }
        }
        if(face == 5) {
            if(doubled) {
                GameState.instance.GetDiceManager().SpawnBigDamageField(myTransform.position);
            } else {
                GameState.instance.GetDiceManager().SpawnSmallDamageField(myTransform.position);
            }
        }
    }
    
    public bool ShouldDamage(int id) {
        return rb.velocity.sqrMagnitude > Mathf.Epsilon && wasHit.Add(id);
    }
    
    public int GetFace() {
        return face;
    }
    
    private int GetFaceFromRotation(float x, float z) {
        if(x == 270 && z == 0) {
            return 1;
        }
        if(x == 0 && z == 0) {
            return 2;
        }
        if(x == 0 && z == 270) {
            return 3;
        }
        if(x == 0 && z == 90) {
            return 4;
        }
        if(x == 0 && z == 180) {
            return 5;
        }
        if(x == 90 && z == 0) {
            return 6;
        }
        return -999;
    }
}
