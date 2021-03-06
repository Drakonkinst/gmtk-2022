using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private const float AngleRotation = 90.0f;
    private const float doublingDistance = 8.0f;
    
    public float activationTime = 1.0f;
    public float disableTime = 0.1f;
    public float damageSpeedMin = 1.0f;
    public float explosionDamage = 100.0f;
    public float smallExplosionRadius = 3.0f;
    public float bigExplosionRadius = 5.0f;
    public int overrideFace = -1;
    public int numSplits = 3;
    public SoundEffect diceRollSound;
    public SoundEffect doubledSound;
    public SoundEffect spawnCardSound;
    public SoundEffect freezeSound;
    public SoundEffect damageSound;
    public SoundEffect healSound;
    public SoundEffect explosionSound;
    public SoundEffect decoySound;
    public GameObject explosionParticle;
    public GameObject freezeParticle;
    public GameObject healParticle;
    public GameObject decoyParticle;
    public GameObject spawnParticle;
    public GameObject damageParticle;
    public GameObject bigExplosionParticle;
    public GameObject bigFreezeParticle;
    public GameObject bigDamageParticle;
    public GameObject bigHealParticle;
    
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
                //Debug.Log(face);
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
        if(face == 1) {
            if(doubled) {
                // Spawn two items
                GameState.instance.SpawnItemDrop(myTransform.position, false);
                GameState.instance.SpawnItemDrop(myTransform.position, false);
            } else {
                // Spawn item
                GameState.instance.SpawnItemDrop(myTransform.position, false);
            }
            GameState.instance.PlaySound(spawnCardSound);
            GameState.instance.PlayEffect(spawnParticle, myTransform.position);
        } else if(face == 2) {
            if(doubled) {
                GameState.instance.GetDiceManager().SpawnBigSlowField(myTransform.position);
                GameState.instance.PlayEffect(bigFreezeParticle, myTransform.position);
            } else {
                GameState.instance.GetDiceManager().SpawnSmallSlowField(myTransform.position);
                GameState.instance.PlayEffect(freezeParticle, myTransform.position);
            }
            GameState.instance.PlaySound(freezeSound);
        } else if(face == 3) {
            if(doubled) {
                GameState.instance.GetDiceManager().SpawnBigHealField(myTransform.position);
                GameState.instance.PlayEffect(bigHealParticle, myTransform.position);
            } else {
                GameState.instance.GetDiceManager().SpawnSmallHealField(myTransform.position);
                GameState.instance.PlayEffect(healParticle, myTransform.position);
            }
            GameState.instance.PlaySound(healSound);
        } else if(face == 4) {
            if(doubled) {
                DoBigExplosion();
                GameState.instance.PlayEffect(bigExplosionParticle, myTransform.position);
            } else {
                DoSmallExplosion();
                GameState.instance.PlayEffect(explosionParticle, myTransform.position);
            }
            GameState.instance.PlaySound(explosionSound);
        } else if(face == 5) {
            if(doubled) {
                GameState.instance.GetDiceManager().SpawnBigDamageField(myTransform.position);
                GameState.instance.PlayEffect(bigDamageParticle, myTransform.position);
            } else {
                GameState.instance.GetDiceManager().SpawnSmallDamageField(myTransform.position);
                GameState.instance.PlayEffect(damageParticle, myTransform.position);
            }
            GameState.instance.PlaySound(damageSound);
        } else if(face == 6) {
            if(doubled) {
                GameState.instance.GetDiceManager().SpawnBigDecoyField(myTransform.position);
            } else {
                GameState.instance.GetDiceManager().SpawnSmallDecoyField(myTransform.position);
            }
            GameState.instance.PlaySound(decoySound);
            GameState.instance.PlayEffect(decoyParticle, myTransform.position);
        }
        if(doubled) {
            GameState.instance.PlaySound(doubledSound);
        }
    }
    
    private void DoSmallExplosion() {
        Transform enemies = GameState.instance.GetEnemySpawner().enemyParent;
        foreach(Transform enemy in enemies) {
            float distSq = (myTransform.position - enemy.position).sqrMagnitude;
            if(distSq <= smallExplosionRadius * smallExplosionRadius) {
                EnemyController enemyData = enemy.GetComponent<EnemyController>();
                enemyData.Damage(explosionDamage);
            }
        }
    }
    
    private void DoBigExplosion() {
        Transform enemies = GameState.instance.GetEnemySpawner().enemyParent;
        foreach(Transform enemy in enemies) {
            float distSq = (myTransform.position - enemy.position).sqrMagnitude;
            if(distSq <= bigExplosionRadius * bigExplosionRadius) {
                EnemyController enemyData = enemy.GetComponent<EnemyController>();
                enemyData.Damage(explosionDamage);
            }
        }
    }
    
    public bool ShouldDamage(int id) {
        //Debug.Log(rb.velocity.magnitude + " " + (rb.velocity.sqrMagnitude > damageSpeedMin * damageSpeedMin));
        return rb.velocity.sqrMagnitude > damageSpeedMin * damageSpeedMin && wasHit.Add(id);
    }
    
    public int GetFace() {
        return face;
    }
    
    private int GetFaceFromRotation(float x, float z) {
        if(overrideFace > 0) {
            return overrideFace;
        }
        
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
    
    void OnCollisionEnter(Collision col) {
        if(col.collider.tag == "Terrain") {
            GameState.instance.PlaySound(diceRollSound);
        }
    }
}
