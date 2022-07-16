using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DiceManager : MonoBehaviour
{   
    public Transform fieldsParent;
    public DamageField smallDamageField;
    public DamageField bigDamageField;
    public HealField smallHealField;
    public HealField bigHealField;
    public SlowField smallSlowField;
    public SlowField bigSlowField;
    public DecoyField smallDecoyField;
    public DecoyField bigDecoyField;
    
    private List<DamageField> damageFields = new List<DamageField>();
    private List<HealField> healFields = new List<HealField>();
    private List<SlowField> slowFields = new List<SlowField>();
    private List<DecoyField> decoyFields = new List<DecoyField>();
    
    void Start()
    {

    }

    void Update()
    {
          
    }
    
    public void SpawnSmallDamageField(Vector3 pos) {
        SpawnField(smallDamageField, pos);
    }
    
    public void SpawnBigDamageField(Vector3 pos) {
        SpawnField(bigDamageField, pos);
    }
    
    public void SpawnSmallHealField(Vector3 pos) {
        SpawnField(smallHealField, pos);
    }
    
    public void SpawnBigHealField(Vector3 pos) {
        SpawnField(bigHealField, pos);
    }
    
    public void SpawnSmallSlowField(Vector3 pos) {
        SpawnField(smallSlowField, pos);
    }
    
    public void SpawnBigSlowField(Vector3 pos) {
        SpawnField(bigSlowField, pos);
    }
    
    public void SpawnSmallDecoyField(Vector3 pos) {
        SpawnField(smallDecoyField, pos);
    }
    
    public void SpawnBigDecoyField(Vector3 pos) {
        SpawnField(bigDecoyField, pos);
    }
    
    public void SpawnField(Field fieldPrefab, Vector3 pos) {
        GameObject obj = Instantiate(fieldPrefab.gameObject, pos, Quaternion.identity, fieldsParent);
        Field field = obj.GetComponent<Field>();
        if(field is DamageField) {
            DamageField damageField = field as DamageField;
            damageFields.Add(damageField);
        } else if(field is HealField) {
            HealField healField = field as HealField;
            healFields.Add(healField);
        } else if(field is SlowField) {
            SlowField slowField = field as SlowField;
            slowFields.Add(slowField);
        } else if(field is DecoyField) {
            DecoyField decoyField = field as DecoyField;
            decoyFields.Add(decoyField);
        }
    }
    
    public void OnFieldExpire(Field field) {
        if(field is DamageField) {
            DamageField damageField = field as DamageField;
            damageFields.Remove(damageField);
        } else if(field is HealField) {
            HealField healField = field as HealField;
            healFields.Remove(healField);
        } else if(field is SlowField) {
            SlowField slowField = field as SlowField;
            slowFields.Remove(slowField);
        } else if(field is DecoyField) {
            DecoyField decoyField = field as DecoyField;
            decoyFields.Remove(decoyField);
        }
    }
    
    public float GetDamageFieldDebuff(Vector3 pos) {
        float sum = 0.0f;
        foreach(DamageField damageField in damageFields) {
            if(damageField.IsInField(pos)) {
                sum += damageField.damagePerSecond;
            }
        }
        return sum;
    }
    
    public (float, float) GetHealFieldBuff(Vector3 pos) {
        float healSum = 0.0f;
        float shieldSum = 0.0f;
        foreach(HealField healField in healFields) {
            if(healField.IsInField(pos)) {
                healSum += healField.hpPerSecond;
                shieldSum += healField.shieldPerSecond;
            }
        }
        return (healSum, shieldSum);
    }
    
    public float GetSlowFieldMultiplier(Vector3 pos) {
        float multiplier = 1.0f;
        foreach(SlowField slowField in slowFields) {
            if(slowField.IsInField(pos)) {
                if(slowField.speedMultiplier < multiplier) {
                    multiplier = slowField.speedMultiplier;
                }
            }
        }
        return multiplier;
    }
    
    public DecoyField GetNearestDecoy(Vector3 pos) {
        float minDistSq = Mathf.Infinity;
        DecoyField nearest = null;
        foreach(DecoyField decoy in decoyFields) {
            Vector3 decoyPos = decoy.transform.position;
            float distSq = (decoyPos - pos).sqrMagnitude;
            if(distSq < decoy.range * decoy.range && distSq < minDistSq) {
                nearest = decoy;
                minDistSq = distSq;
            }   
        }
        return nearest;
    }
}
