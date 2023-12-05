using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Ensures that the player script is on the game object as a requirement.
[RequireComponent(typeof(Player))]
public class PlayerSprinting : MonoBehaviour
{
    [SerializeField] float speedMultiplier = 2f;

    // References to player script. 
    Player player;
    PlayerInput playerInput;
    InputAction sprintAction;

    private void Awake()
    {
        // References to the player input component.
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        sprintAction = playerInput.actions["sprint"];
    }

    // Registers the OnBeforeMove method.
    void OnEnable() => player.OnBeforeMove += OnBeforeMove;
    // Unregisters it. 
    void OnDisable() => player.OnBeforeMove -= OnBeforeMove;

    void OnBeforeMove()
    {
        // Reads the sprint action value. 
        var sprintInput = sprintAction.ReadValue<float>();

        // If it's more than zero, then multiply it by the multiplier to enable sprinting. 
        player.movementSpeedMultiplier *= sprintInput > 0 ? speedMultiplier : 1f;
    }
}
