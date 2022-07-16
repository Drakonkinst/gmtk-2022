using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public HUDMeter healthBar;
    public HUDMeter shieldBar;
    public float fireInterval = 0.2f;
    public float maxHealth = 100.0f;
    public float startingShield = 0.0f;
    public float healthRegenDelay = 5.0f;
    public float healthRegenPerSecond = 5.0f;
    
    private Transform myTransform;
    private DiceThrower thrower;
    private PlayerController movement;
    private float health;
    private float shield;
    private float nextHealthRegenDelay;
    
    void Awake() {
        health = maxHealth;
        shield = startingShield;
        myTransform = transform;
        thrower = GetComponent<DiceThrower>();
        movement = GetComponent<PlayerController>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateHealthBar();
        UpdateShieldBar();
    }

    // Update is called once per frame
    void Update()
    {
        if(health < maxHealth && Time.time > nextHealthRegenDelay) {
            Heal(healthRegenPerSecond * Time.deltaTime);
        }
    }
    
    public void Damage(float amount) {
        if(shield >= amount) {
            shield -= amount;
            UpdateShieldBar();
        } else if(shield > 0) {
            amount -= shield;
            shield = 0.0f;
            UpdateShieldBar();
        }
        if(amount > 0) {
            health -= amount;
            if(health <= 0) {
                health = 0.0f;
                // Dead
            }
            UpdateHealthBar();
            nextHealthRegenDelay = Time.time + healthRegenDelay;
        }
    }
    
    public void Heal(float amount, bool overflow = false) {
        health += amount;
        if(health > maxHealth) {
            if(overflow) {
                shield = Mathf.Clamp(maxHealth - health, 0.0f, maxHealth);
                UpdateShieldBar();
            }
            health = maxHealth;
        }
        UpdateHealthBar();
    }
    
    private void UpdateHealthBar() {
        healthBar.SetPercentage(health / maxHealth);
    }
    
    private void UpdateShieldBar() {
        shieldBar.SetPercentage(shield / maxHealth);
    }
    
    public Transform GetTransform() {
        return myTransform;
    }
    
    public PlayerController GetMovement() {
        return movement;
    }
    
    public DiceThrower GetThrower() {
        return thrower;
    }
}
