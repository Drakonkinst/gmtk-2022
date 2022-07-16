using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public struct ItemEntry {
    public string id;
    public string description;
    public float weight;
    public bool stackable;
    // TODO: Icon prefab
}

[RequireComponent(typeof(EnemySpawner))]
[RequireComponent(typeof(DiceManager))]
public class GameState : MonoBehaviour
{
    public static GameState instance;
    
    public Player player;
    public Transform diceParent;
    public Camera mainCamera;
    public Renderer groundRenderer;
    public ItemEntry[] itemRegistry;
    
    private EnemySpawner enemySpawner;
    private DiceManager diceManager;
    private Vector3 groundCenter;
    private float groundWidth;
    private float groundHeight;
    private int walkableMask;
    
    void Awake() {
        instance = this;
        enemySpawner = GetComponent<EnemySpawner>();
        diceManager = GetComponent<DiceManager>();

        Bounds bounds = groundRenderer.bounds;
        groundCenter = bounds.center;
        groundWidth = bounds.extents.x * 2;
        groundHeight = bounds.extents.z * 2;
        walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
    }

    public Player GetPlayer() {
        return player;
    }
    
    public Camera GetMainCamera() {
        return mainCamera;
    }
    
    public EnemySpawner GetEnemySpawner() {
        return enemySpawner;
    }
    
    public Vector3 RandomGroundPoint() {
        // Generate random possible point
        float x = UnityEngine.Random.Range(groundCenter.x - groundWidth / 2, groundCenter.x + groundWidth / 2);
        float z = UnityEngine.Random.Range(groundCenter.z - groundHeight / 2, groundCenter.z + groundHeight / 2);
        Vector3 pos = new Vector3(x, groundCenter.y, z);
        
        // Get closest point on NavMesh
        NavMeshHit hit;
        if(NavMesh.SamplePosition(pos, out hit, 10.0f, walkableMask)) {
            return hit.position;
        }
        Debug.LogWarning("Failed to find a navmesh position near " + pos);
        return pos;
    }
    
    public float GetGroundLength() {
        return groundHeight;
    }
    
    public DiceManager GetDiceManager() {
        return diceManager;
    }
    
    public Transform GetDiceParent() {
        return diceParent;
    }
    
    public ItemEntry GetItemInfo(string id) {
        foreach(ItemEntry item in itemRegistry) {
            if(item.id == id) {
                return item;
            }
        }
        Debug.LogWarning("Unknown item " + id);
        return new ItemEntry();
    }
}
