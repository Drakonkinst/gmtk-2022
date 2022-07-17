using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Rigidbody))]
public class DiceThrower : MonoBehaviour
{
    private const float minGamepadThreshold = 0.7f;
    public GameObject dicePrefab;
    public float spawnOffsetForward = 5;
    public float spawnOffsetUp = 0;
    public float throwSpeedForward = 500.0f;
    public float throwSpeedUp = 100.0f;
    public float momentumMultiplier = 40.0f;
    public float tumble = 50.0f;
    public SoundEffect throwDiceSound;
    
    // Upgrades
    public float bonusThrowSpeed = 100.0f;
    public float bonusThrowRateMultiplier = 0.25f;
    
    private Transform myTransform;
    private PlayerController movement;
    private Player player;
    private Rigidbody rb;
    private bool isMouseHeld = false;
    private Vector2 gamepadInput = new Vector2();
    private float nextFire = 0;
    
    void Awake() {
        myTransform = transform;
        movement = GetComponent<PlayerController>();
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool shouldFire = isMouseHeld || gamepadInput.sqrMagnitude >= minGamepadThreshold * minGamepadThreshold;
        if(player.CanShootDice() && shouldFire && nextFire < Time.time) {
            Vector3 dir = FindTargetDir();
            Fire(myTransform.position, dir, true);
            GameState.instance.PlaySound(throwDiceSound);
            int numDoubleShot = player.GetCount("DoubleShot");
            while(numDoubleShot > 0) {
                Fire(myTransform.position, dir, false);
                numDoubleShot--;
            }
        
            int quickShotBonus = player.GetCount("QuickShot");
            float delay = Mathf.Clamp(player.fireInterval * Mathf.Clamp01(1.0f - bonusThrowRateMultiplier * quickShotBonus), 0.1f, Mathf.Infinity);
            if(!player.CanShootDice()) {
                delay += player.firePenaltyDelay;
            }
            nextFire = Time.time + delay;
        }
    }
    
    public void OnFire(InputValue value) {
        if(player.isDead) {
            return;
        }
        isMouseHeld = value.isPressed;
    }
    
    public void OnFireDirectional(InputValue value) {
        if(player.isDead) {
            return;
        }
        gamepadInput = value.Get<Vector2>();
    }
    
    private Vector3 FindTargetDir() {
        string controls = GameState.instance.GetPlayer().GetComponent<PlayerInput>().currentControlScheme;
        if(controls == "Keyboard&Mouse") {
            Vector3 mousePos = GameState.instance.GetMainCamera().ScreenToWorldPoint(Mouse.current.position.ReadValue());
            return new Vector3(mousePos.x - myTransform.position.x, 0.0f, mousePos.z - myTransform.position.z).normalized;
        } else if(controls == "Gamepad" && gamepadInput != Vector2.zero) {
            return new Vector3(gamepadInput.x, 0.0f, gamepadInput.y).normalized;
        } else {
            return myTransform.forward;
        }
    }
    
    // targetDir must be NORMALIZED
    public void Fire(Vector3 position, Vector3 targetDir, bool consumeDice, int numSplits = -1) {
        Vector3 spawnPos = position + targetDir * spawnOffsetForward + new Vector3(0.0f, spawnOffsetUp, 0.0f);
        GameObject dice = Instantiate(dicePrefab, spawnPos, Quaternion.LookRotation(targetDir, Vector3.up), GameState.instance.GetDiceParent());
        Rigidbody diceRb = dice.GetComponent<Rigidbody>();
        // -1 is default
        if(numSplits > -1) {
            dice.GetComponent<Dice>().numSplits = numSplits;
        }
        
        float forwardSpeed = throwSpeedForward + player.GetCount("LongShot") * bonusThrowSpeed;
        if(movement.IsMoving()) {
            forwardSpeed += movement.GetMaxSpeed() * momentumMultiplier;
        }
        Vector3 velocity = targetDir * forwardSpeed + dice.transform.up * throwSpeedUp;
        diceRb.angularVelocity = Random.insideUnitSphere * tumble;
        diceRb.rotation = Quaternion.Euler(new Vector3(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f)));
        diceRb.AddForce(velocity);
        if(consumeDice) {
            player.OnShootDice();
        }
    }
}
