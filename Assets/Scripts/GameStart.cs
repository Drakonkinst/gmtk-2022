using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{   
    void Start() {
        Time.timeScale = 0.0f;
        GameState.instance.gameStarted = false;
        GameState.instance.GetPlayer().isDead = true;
    }
    
    public void StartGame() {
        Time.timeScale = 1.0f;
        GameState.instance.gameStarted = true;
        GameState.instance.GetPlayer().isDead = false;
        gameObject.SetActive(false);
    }
}
