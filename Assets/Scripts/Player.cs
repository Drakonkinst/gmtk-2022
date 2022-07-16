using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public HUDMeter healthBar;
    public HUDMeter shieldBar;
    public DiceIcon diceIcon;
    public float fireInterval = 0.2f;
    public float maxHealth = 100.0f;
    public float startingShield = 0.0f;
    public float healthRegenDelay = 5.0f;
    public float healthRegenPerSecond = 5.0f;
    public float ammoRegenDelay = 1.0f;
    public float ammoRegenPenalty = 0.25f;
    public float firePenaltyDelay = 0.5f;
    public int maxAmmo = 4;
    
    private Transform myTransform;
    private DiceThrower thrower;
    private PlayerController movement;
    protected DiceManager diceManager;
    private float health;
    private float shield;
    private float nextHealthRegenDelay;
    private int ammo;
    private float nextAmmoRegen;
    
    void Awake() {
        health = maxHealth;
        shield = startingShield;
        myTransform = transform;
        thrower = GetComponent<DiceThrower>();
        movement = GetComponent<PlayerController>();
        ammo = maxAmmo;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateHealthBar();
        UpdateShieldBar();
        diceIcon.SetFaceInstant(ammo);
        diceManager = GameState.instance.GetDiceManager();
    }

    // Update is called once per frame
    void Update()
    {
        if(health < maxHealth && Time.time > nextHealthRegenDelay) {
            Heal(healthRegenPerSecond * Time.deltaTime);
        }
        
        float hpRegen = diceManager.GetHealFieldBuff(myTransform.position);
        if(hpRegen > 0) {
            Heal(hpRegen * Time.deltaTime);
        }
        
        if(Time.time > nextAmmoRegen) {
            if(ammo < maxAmmo) {
                ammo++;
                diceIcon.SetFace(ammo);
            }
            if(ammo < maxAmmo) {
                nextAmmoRegen = Time.time + ammoRegenDelay;
            }
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
    
    public void OnShootDice() {
        ammo--;
        if(ammo < 0) {
            ammo = 0;
        }
        diceIcon.SetFace(ammo);
        float nextRegen = Time.time + ammoRegenDelay;
        if(ammo == 0) {
            nextAmmoRegen = nextRegen + ammoRegenPenalty;
        } else {
            nextAmmoRegen = nextRegen;
        }
    }
    
    private void UpdateHealthBar() {
        healthBar.SetPercentage(health / maxHealth);
    }
    
    private void UpdateShieldBar() {
        shieldBar.SetPercentage(shield / maxHealth);
    }
    
    public bool CanShootDice() {
        return ammo > 0;
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
