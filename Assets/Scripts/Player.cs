using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public HUDMeter healthBar;
    public HUDMeter shieldBar;
    public DiceIcon diceIcon;
    public Hotbar hotbar;
    public float fireInterval = 0.2f;
    public float maxHealth = 100.0f;
    public float startingShield = 0.0f;
    public float healthRegenDelay = 5.0f;
    public float healthRegenPerSecond = 5.0f;
    public float ammoRegenDelay = 1.0f;
    public float ammoRegenPenalty = 0.25f;
    public float firePenaltyDelay = 0.5f;
    public int maxAmmo = 4;
    public int bonusMaxAmmo = 2;
    public float bonusReloadMultiplier = 0.3f;
    public float undyingHeal = 60.0f;
    public float undyingShield = 30.0f;
    public string[] inventory;
    
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
        if(inventory == null) {
            inventory = new string[4];
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateHealthBar();
        UpdateShieldBar();
        diceIcon.SetFaceInstant(ammo);
        diceManager = GameState.instance.GetDiceManager();
        UpdateHotbar();
    }

    // Update is called once per frame
    void Update()
    {
        if(health < maxHealth && Time.time > nextHealthRegenDelay) {
            Heal(healthRegenPerSecond * Time.deltaTime);
        }
        
        (float hpRegen, float shieldRegen) = diceManager.GetHealFieldBuff(myTransform.position);
        if(hpRegen > 0) {
            Heal(hpRegen * Time.deltaTime);
        }
        if(shieldRegen > 0) {
            AddShield(shieldRegen * Time.deltaTime);
        }
        
        if(Time.time > nextAmmoRegen) {
            if(ammo < GetMaxAmmo()) {
                ammo++;
                diceIcon.SetFace(ammo);
            }
            if(ammo < GetMaxAmmo()) {
                SetNextAmmoRegen();
            }
        }
    }
    
    public void Damage(float amount) {
        if(shield >= amount) {
            shield -= amount;
            amount = 0.0f;
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
                if(HasItem("Undying")) {
                    Heal(undyingHeal);
                    AddShield(undyingShield);
                    RemoveItem("Undying");
                } else {
                    // Dead
                }
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
    
    public void AddShield(float amount) {
        shield += amount;
        if(shield > maxHealth) {
            shield = maxHealth;
        }
        UpdateShieldBar();
    }
    
    public void OnShootDice() {
        ammo--;
        if(ammo < 0) {
            ammo = 0;
        }
        diceIcon.SetFace(ammo);
        SetNextAmmoRegen();
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
    
    private void SelectIfValid(int index) {
        if(hotbar.IsValid(index)) {
            hotbar.Select(index);
        }
    }
    
    public int GetMaxAmmo() {
        if(HasItem("MoreAmmo")) {
            return maxAmmo + bonusMaxAmmo;
        }
        return maxAmmo;
    }
    
    private void SetNextAmmoRegen() {
        float delay = ammoRegenDelay;
        if(ammo == 0) {
            delay += ammoRegenPenalty;
        }
        int reloadBonus = GetCount("FastReload");
        delay *= Mathf.Clamp(1.0f - bonusReloadMultiplier * reloadBonus, 0.4f, 1.0f);
        nextAmmoRegen = Time.time + delay;
    }
    
    // Input
    
    public void OnScroll(InputValue value) {
        float scroll = value.Get<Vector2>().y;
        if(scroll > 0) {
            hotbar.SelectPrev();
        } else if(scroll < 0) {
            hotbar.SelectNext();
        }
    }
    
    public void OnSelectItem1() {
        SelectIfValid(0);
    }
    
    public void OnSelectItem2() {
        SelectIfValid(1);
    }
    
    public void OnSelectItem3() {
        SelectIfValid(2);
    }
    
    public void OnSelectItem4() {
        SelectIfValid(3);
    }
    
    public void OnSelectNext() {
        hotbar.SelectNext();
    }
    
    public void OnSelectPrev() {
        hotbar.SelectPrev();
    }
    
    // Items
    
    public bool HasItem(string id) {
        return GetIndex(id) > -1;
    }
    
    public int GetCount(string id) {
        int sum = 0;
        foreach(string item in inventory) {
            if(item == id) {
                sum++;
            }
        }
        return sum;
    }
    
    public bool HasOpenSlot() {
        foreach(string item in inventory) {
            if(item == null || item.Length <= 0) {
                return true;
            }
        }
        return false;
    }
    
    private int GetNextOpenSlot() {
        for(int i = 0; i < inventory.Length; ++i) {
            if(inventory[i] == null || inventory[i].Length <= 0) {
                return i;
            }
        }
        return -1;
    }
    
    public int GetIndex(string id) {
        for(int i = 0; i < inventory.Length; ++i) {
            if(inventory[i] == id) {
                return i;
            }
        }
        return -1;
    }
    
    public bool AddItem(string id) {
        int nextOpenSlot = GetNextOpenSlot();
        if(nextOpenSlot < 0) {
            return false;
        }
        inventory[nextOpenSlot] = id;
        UpdateHotbar();
        return true;
    }
    
    public bool RemoveItem(string id) {
        int index = GetIndex(id);
        if(index < 0) {
            return false;
        }
        inventory[index] = null;
        UpdateHotbar();
        return true;
    }
    
    private void UpdateHotbar() {
        hotbar.SetContents(inventory);
    }
}
