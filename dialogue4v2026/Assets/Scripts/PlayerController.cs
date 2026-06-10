using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;

    [Header("Movement")]
    [Tooltip("Acceleration applied to the rigidbody when input is received (units/s^2)")]
    public float moveAcceleration = 10f;

    [Tooltip("Maximum horizontal speed (m/s). Set to <= 0 to disable clamping.")]
    public float maxSpeed = 6f;

    private bool m_IsInteracting;

    Rigidbody m_Rigidbody;
    Vector2 m_MoveInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        m_Rigidbody = GetComponent<Rigidbody>();
        if (m_Rigidbody == null)
            Debug.LogError("PlayerController requires a Rigidbody on the same GameObject.");
    }

    void OnEnable()
    {
        playerInput.actions.FindAction("Move").performed += OnMovePerformed;
        playerInput.actions.FindAction("Move").canceled += 
            context => {m_MoveInput = Vector2.zero;};

        playerInput.actions.FindAction("Interact").performed += OnInteract;
    }

    

    void OnDisable()
    {
        playerInput.actions.FindAction("Move").performed -= OnMovePerformed;
        playerInput.actions.FindAction("Interact").performed -= OnInteract;
    }

    void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        m_MoveInput = ctx.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (m_Rigidbody == null)
            return;

        if (m_IsInteracting)
        {
            m_Rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        // Convert 2D input (x,y) to world X,Z movement
        Vector3 desired = new Vector3(m_MoveInput.x, 0f, m_MoveInput.y);

        if (desired.sqrMagnitude > 0f)
        {
            Vector3 accel = desired.normalized * moveAcceleration;
            // Use acceleration so movement feels consistent across masses
            m_Rigidbody.AddForce(accel, ForceMode.Acceleration);
        }

        // Optional: clamp horizontal velocity
        if (maxSpeed > 0f)
        {
            Vector3 horizontalVel = new Vector3(m_Rigidbody.linearVelocity.x, 0f, m_Rigidbody.linearVelocity.z);
            float speed = horizontalVel.magnitude;
            if (speed > maxSpeed)
            {
                Vector3 limited = horizontalVel.normalized * maxSpeed;
                m_Rigidbody.linearVelocity = new Vector3(limited.x, m_Rigidbody.linearVelocity.y, limited.z);
            }
        }
    }
    
    private void OnInteract(InputAction.CallbackContext obj)
    {
        InteractOM.PlayerInteracted(ref m_IsInteracting);
    }
}

