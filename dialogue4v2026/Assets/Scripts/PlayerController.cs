using UnityEngine;
using UnityEngine.InputSystem;
using Core;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Drag the Move action (Vector2) from the Input System asset here (use an Input Action Reference)")]
    public InputActionReference moveAction;

    [Header("Movement")]
    [Tooltip("Acceleration applied to the rigidbody when input is received (units/s^2)")]
    public float moveAcceleration = 10f;

    [Tooltip("Maximum horizontal speed (m/s). Set to <= 0 to disable clamping.")]
    public float maxSpeed = 6f;

    Rigidbody m_Rigidbody;
    Vector2 m_MoveInput;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        if (m_Rigidbody == null)
            Debug.LogError("PlayerController requires a Rigidbody on the same GameObject.");
    }

    void OnEnable()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.Enable();
            moveAction.action.performed += OnMovePerformed;
            moveAction.action.canceled += OnMovePerformed;
        }
    }

    void OnDisable()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.performed -= OnMovePerformed;
            moveAction.action.canceled -= OnMovePerformed;
            moveAction.action.Disable();
        }
    }

    void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        if (!GameManager.Instance.IsInGameplay())
            return;
        m_MoveInput = ctx.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (m_Rigidbody == null)
            return;

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
}
