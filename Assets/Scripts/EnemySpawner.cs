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
        public float minSurvivalTime;
    }

    public int maxStrength = 5;
    public float currentStrength = 0;
    public float spawnInterval = 5.0f;
    public EnemyEntry[] enemies;
    public Transform enemyParent;
    public float difficultyIncreaseInterval = 10.0f;
    
    private float nextSpawn;
    private float weightSum = 0.0f;
    private float nextIncrease;
    
    void Awake() {
        foreach(EnemyEntry entry in enemies) {
            weightSum += entry.weight;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        DoSpawns();
        nextSpawn = Time.time + spawnInterval;
        nextIncrease = Time.time + difficultyIncreaseInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextSpawn) {
            DoSpawns();
            nextSpawn = Time.time + spawnInterval;
        }    
        if(Time.time > nextIncrease) {
            ++maxStrength;
            Debug.Log("DIFFICULTY " + maxStrength);
            nextIncrease = Time.time + difficultyIncreaseInterval;
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
        // Pick enemy type
        int attemptsRemaining = 3;
        EnemyEntry entry = RandomEnemy();
        while(entry.minSurvivalTime > GameState.instance.GetTimeSurvived() && attemptsRemaining-- > 0) {
            entry = RandomEnemy();
        }
        
        // Pick position
        // TODO: Make sure it is not close to the player
        Vector3 position = GameState.instance.RandomGroundPoint();
        position = new Vector3(position.x, 1.0f, position.z);
        //Debug.Log("Spawning enemy at " + position);
        
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
    
    public EnemyEntry RandomEnemy() {
        float target = UnityEngine.Random.Range(0.0f, weightSum);
        float curr = 0.0f;
        int index = 0;
        while(curr <= target) {
            EnemyEntry entry = enemies[index++];
            curr += entry.weight;
            if(curr >= target) {
                return entry;
            }
        }
        Debug.LogWarning("Failed to generate item");
        return new EnemyEntry();
    }
}
