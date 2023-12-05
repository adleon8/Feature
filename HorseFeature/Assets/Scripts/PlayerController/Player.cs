using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 30f;
    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float mass = 1f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] Transform cameraTransform;

    // Allows it to be accessed but doesn't expose controller.
    public bool IsGrounded => controller.isGrounded;

    // Sets player's height.
    public float Height
    {
        get => controller.height;
        set => controller.height = value;
    }
    
    // Sets player's radius.
    public float Radius
    {
        get => controller.radius;
        set => controller.radius = value;
    }

    public event Action OnBeforeMove;
    public event Action<bool> OnGroundStateChange; // Takes a boolean 'cause it is the current ground state.

    // Visible only to the current assembly.
    internal float movementSpeedMultiplier;

    // To have the ability to look around.
    Vector2 look;
    // Character controler reference.
    CharacterController controller;
    // Player's velocity.
    internal Vector3 velocity;

    // For grounding.
    bool wasGrounded;

    // Input system.
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction sprintAction;

    private void Awake()
    {
        // References the character component in the game object.
        controller = GetComponent<CharacterController>();

        // References the player input actions.
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["look"];
        //jumpAction = playerInput.actions["jump"];
        sprintAction = playerInput.actions["sprint"];

    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        GroundUpdate();
        Gravity();
        Movement();
        Look();
        
    }
    
    private void SlopeSliding()
    {
        if (IsGrounded)
        {
            // Calculates center of the sphere
            //var
        }
    }

    private void GroundUpdate()
    {
        SlopeSliding();
        if (wasGrounded != IsGrounded)
        {
            OnGroundStateChange?.Invoke(IsGrounded);
            wasGrounded = IsGrounded;
        }
    }

    private void Gravity()
    {
        // Creates gravity variable.
        var gravity = Physics.gravity * mass * Time.deltaTime;

        // Determines vertical velocity.
        velocity.y = IsGrounded ? -1f : velocity.y + gravity.y;
    }

    Vector3 GetMovementInput()
    {
        // Takes the input and reads the value methods of moving actions.
        var moveInput = moveAction.ReadValue<Vector2>();

        // Input Vector.
        var input = new Vector3();

        // Adds vector pointing forward. 
        input += transform.forward * moveInput.y;

        // Adds vector pointing to the right.
        input += transform.right * moveInput.x;

        // Clamping it to 1 so the vector doesn't move faster diagonally than normally. 
        input = Vector3.ClampMagnitude(input, 1f);

        // Applies the multiplier to the normal movement speed to increase it and sprint. 
        input *= movementSpeed * movementSpeedMultiplier;

        // Takes in player's input and activates a smoother movement.
        input *= movementSpeed;

        return input;
    }

    private void Movement()
    {
        // Sets the value for the multiplier.
        movementSpeedMultiplier = 1.5f;
        
        // Allows external scripts to have access to the player controller's cycle. 
        OnBeforeMove?.Invoke();

        var input = GetMovementInput();

        // Uses acceleration to prevent player's movement from looking too abrupt.
        // Gradually interpolates back to zero or up to the input being pressed.
        var factor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);

        // Moves the player but uses the controller instead.
        controller.Move(velocity * Time.deltaTime);
    }

    private void Look()
    {
        // Takes player input for looking.
        var lookInput = lookAction.ReadValue<Vector2>();

        // Checks where the mouse position is going on the X and Y axis.
        look.x += Input.GetAxis("Mouse X");
        look.y += Input.GetAxis("Mouse Y");

        // Does not allow the player to do a 360 rotation.
        look.y = Mathf.Clamp(look.y, -89f, 89f);

        // Debugger to check if coordinates are changing.
        //Debug.Log(look);

        // Changes the rotation of the transform.
        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);

        // Quaternion to modify rotation.
        transform.localRotation = Quaternion.Euler(0, look.x, 0);
    }
}
