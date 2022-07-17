using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    private static Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0.0f, 45.0f, 0.0f));
    
    public float speed = 10;
    public float turnSpeed = 360;
    public float bonusSpeedMultiplier = 0.2f;
    public Animator animator;
    
    private CharacterController controller;
    private Player player;
    private Vector3 movementVelocity;
    private Vector2 moveInput;
    private Transform myTransform;
    private AudioSource footstepSource;
    private float yPos;
    private bool isPlayingSound = false;
    
    void Awake() {
        controller = GetComponent<CharacterController>();
        player = GetComponent<Player>();
        myTransform = transform;
        yPos = myTransform.position.y;
        footstepSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 inputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;
        if(inputDirection != Vector3.zero && !player.isDead) {
            Look(inputDirection);
            movementVelocity = myTransform.forward * GetMaxSpeed() * Time.deltaTime;
            animator.SetBool("Walk_Anim", true);
            if(!isPlayingSound) {
                footstepSource.Play();
                isPlayingSound = true;
            }
            
        } else {
            movementVelocity.x = 0.0f;
            movementVelocity.z = 0.0f;
            animator.SetBool("Walk_Anim", false);
            if(isPlayingSound) {
                footstepSource.Stop();
                isPlayingSound = false;
            }
        }
        controller.Move(movementVelocity);
        myTransform.position = new Vector3(myTransform.position.x, yPos, myTransform.position.z);
    }
    
    void Look(Vector3 inputDirection) {
        // Isometric: https://www.youtube.com/watch?v=8ZxVBCvJDWk
        //Vector3 skewedInput = isometricMatrix.MultiplyPoint3x4(inputDirection);
        Vector3 relative = (myTransform.position + inputDirection) - myTransform.position;
        Quaternion rotation = Quaternion.LookRotation(relative, Vector3.up);
        
        // Instant vs smooth rotation
        myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, rotation, turnSpeed * Time.deltaTime);
        //myTransform.rotation = rotation;
    }
    
    public Vector3 GetMovementVelocity() {
        return movementVelocity;
    }
    
    public bool IsMoving() {
        return movementVelocity != Vector3.zero;
    }
    
    public float GetMaxSpeed() {
        return speed * (1.0f + bonusSpeedMultiplier * player.GetCount("FastRun"));
    }
    
    // Input Actions
    
    public void OnMove(InputValue value) {
        if(player.isDead) {
            return;
        }
        moveInput = value.Get<Vector2>();
    }
}
