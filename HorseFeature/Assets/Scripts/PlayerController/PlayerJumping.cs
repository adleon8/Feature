using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]

public class PlayerJumping : MonoBehaviour
{
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float jumpPressBufferTime = .01f;
    [SerializeField] float jumpGroundGraceTime = .02f; // This and all the grace time mechanics are added so that player doesn't have to time their jumping off ledges. 

    Player player;

    // Variable to check if player is tryint to or not. 
    bool tryingToJump;
    float lastJumpPressTime;
    float lastGroundedTime;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void OnEnable()
    {
        player.OnBeforeMove += OnBeforeMove;
        player.OnGroundStateChange += OnGroundStateChange;
    }

    void OnDisable()
    {
        player.OnBeforeMove -= OnBeforeMove;
    }

    public void OnJump(InputAction.CallbackContext context) // This determines the context of the press, checking if it was starting to press, druing, after.
    {
        if (context.performed)
        {
            tryingToJump = true;
            // Sets it to the current time.
            lastJumpPressTime = Time.time;
        }
        
        if (context.canceled)
        {
            tryingToJump = false;
        }
    }

    void OnBeforeMove()
    {
        // Checks if the player tried to jump no longer that jump press buffer time.
        bool wasTryingToJump = Time.time - lastJumpPressTime < jumpPressBufferTime;
        //Debug.Log(wasTryingToJump);
        
        // Checks if the player was just grounded.
        bool wasGrounded = Time.time - lastGroundedTime < jumpGroundGraceTime;
        //Debug.Log(tryingToJump);

        
        // then check if player tried to jump recently.
        bool isOrWasTryingToJump = tryingToJump || (wasTryingToJump && player.IsGrounded);
        Debug.Log(isOrWasTryingToJump);
        
        // Checks if both.
        bool isOrWasGrounded = player.IsGrounded || wasGrounded;

        if (isOrWasTryingToJump && isOrWasGrounded)
        {
            Debug.Log("Lol");
            player.velocity.y += jumpSpeed;
        }

        tryingToJump = false; 
    }

    void OnGroundStateChange(bool isGrounded)
    {
        if (!isGrounded) lastGroundedTime = Time.time;
    }
}
