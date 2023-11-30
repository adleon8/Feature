using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 7f;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float mass = 1f;
    [SerializeField] Transform cameraTransform;

    // To have the ability to look around.
    Vector2 look;
    // Character controler reference.
    CharacterController controller;
    // Player's velocity.
    Vector3 velocity;

    private void Awake()
    {
        // References the character component in the game object.
        controller = GetComponent<CharacterController>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Movement();
        Look();
        Gravity();
    }

    private void Gravity()
    {
        // Creates gravity variable.
        var gravity = Physics.gravity * mass * Time.deltaTime;

        // Determines vertical velocity.
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
    }

    private void Movement()
    {
        // Takes the input of the horizontal and vertical axis.
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        // Input Vector.
        var input = new Vector3();

        // Adds vector pointing forward. 
        input += transform.forward * y;

        // Adds vector pointing to the right.
        input += transform.right * x;

        // Clamping it to 1 so the vector doesn't move faster diagonally than normally. 
        input = Vector3.ClampMagnitude(input, 1f);

        // Moves the player by calling transform translate. Makes the movement not frame rate dependant.
        //transform.Translate(input * movementSpeed * Time.deltaTime, Space.World);

        // Allows jumping.
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            // Add the jumping height to the y axis velocity to lift player off the ground.
            velocity.y += jumpSpeed;
        }

        // Moves the player but uses the controller instead.
        controller.Move((input * movementSpeed + velocity) * Time.deltaTime);
    }

    private void Look()
    {
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
