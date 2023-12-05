using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 7f;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float mass = 1f;
    [SerializeField] float acceleration = 20f;
    [SerializeField] Transform cameraTransform;

    // To have the ability to look around.
    Vector2 look;
    // Character controler reference.
    CharacterController controller;
    // Player's velocity.
    Vector3 velocity;

    // Input system.
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;

    private void Awake()
    {
        // References the character component in the game object.
        controller = GetComponent<CharacterController>();

        // References the player input actions.
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["look"];
        jumpAction = playerInput.actions["jump"];

    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Movement();
        Look();
        Gravity();
    }
    
    // STAY HERE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private void SlopeSliding()
    {
        //if (Is)
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
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
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

        // Takes in player's input and activates a smoother movement.
        input *= movementSpeed;

        return input;
    }

    private void Movement()
    {
        var input = GetMovementInput();

        // Uses acceleration to prevent player's movement from looking too abrupt.
        // Gradually interpolates back to zero or up to the input being pressed.
        var factor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);

        // Allows jumping.
        var jumpInput = jumpAction.ReadValue<float>();
        if (jumpInput > 0 && controller.isGrounded)
        {
            // Add the jumping height to the y axis velocity to lift player off the ground.
            velocity.y += jumpSpeed;
        }

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
