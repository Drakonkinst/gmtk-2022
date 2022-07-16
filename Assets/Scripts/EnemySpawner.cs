using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public struct EnemyEntry {
        public string id;
        public GameObject prefab;
        public float weight;
        public int strength;
    }

    public int maxStrength = 5;
    public float currentStrength = 0;
    public float spawnInterval = 5.0f;
    public EnemyEntry[] enemies;
    public Transform enemyParent;
    
    private float nextSpawn;
    
    // Start is called before the first frame update
    void Start()
    {
        DoSpawns();
        nextSpawn = Time.time + spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextSpawn) {
            DoSpawns();
            nextSpawn = Time.time + spawnInterval;
        }        
    }
    
    public void OnDeath(string id) {
        EnemyEntry entry = GetEntryById(id);
        if(entry.id == null) {
            Debug.LogWarning("Unknown enemy \"" + id + "\"");
            return;
        }
    }
    
    private void DoSpawns() {
        UpdateStrength();
        while(currentStrength < maxStrength) {
            // This may cause it to overflow maxStrength a bit but that's okay
            // It's more of a guideline anyways
            DoSpawn();
        }
    }
    
    private void DoSpawn() {
        // Hardcoded to single enemy
        EnemyEntry entry = enemies[0];
        
        // Pick position
        // TODO: Make sure it is not close to the player
        Vector3 position = GameState.instance.RandomGroundPoint();
        Debug.Log("Spawning enemy at " + position);
        
        // Spawn enemy
        Instantiate(entry.prefab, position, Quaternion.identity, enemyParent);
        currentStrength += entry.strength;
    }
    
    private void UpdateStrength() {
        int sum = 0;
        foreach(Transform child in enemyParent)
        {
            string id = child.gameObject.GetComponent<EnemyController>().id;
            EnemyEntry entry = GetEntryById(id);
            sum += entry.strength;
        }
        currentStrength = sum;
    }
    
    private EnemyEntry GetEntryById(string id) {
        foreach(EnemyEntry entry in enemies) {
            if(entry.id == id) {
                return entry;
            }
        }
        return new EnemyEntry();
    }
}
