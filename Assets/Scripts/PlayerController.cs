using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private static Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0.0f, 45.0f, 0.0f));
    
    public float speed = 10;
    public float turnSpeed = 360;
    
    private CharacterController controller;
    private Vector3 movementVelocity;
    private Vector2 moveInput;
    private Transform myTransform;
    private float yPos;
    
    void Awake() {
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        yPos = myTransform.position.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 inputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;
        if(inputDirection != Vector3.zero) {
            Look(inputDirection);
            movementVelocity = myTransform.forward * speed * Time.deltaTime;
        } else {
            movementVelocity.x = 0.0f;
            movementVelocity.z = 0.0f;
        }
        controller.Move(movementVelocity);
        myTransform.position = new Vector3(myTransform.position.x, yPos, myTransform.position.z);
    }
    
    void Look(Vector3 inputDirection) {
        Vector3 skewedInput = isometricMatrix.MultiplyPoint3x4(inputDirection);
        Vector3 relative = (myTransform.position + skewedInput) - myTransform.position;
        Quaternion rotation = Quaternion.LookRotation(relative, Vector3.up);
        myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, rotation, turnSpeed * Time.deltaTime);
    }
    
    public Vector3 GetMovementVelocity() {
        return movementVelocity;
    }
    
    public bool IsMoving() {
        return movementVelocity != Vector3.zero;
    }
    
    public float GetMaxSpeed() {
        return speed;
    }
    
    // Input Actions
    
    public void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }
}