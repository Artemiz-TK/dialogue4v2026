using UnityEngine;
using UnityEngine.InputSystem;
using Core;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Drag the Move action (Vector2) from the Input System asset here (use an Input Action Reference)")]
    public readonly InputActionReference m_Asset;

    [Header("Movement")]
    [Tooltip("Acceleration applied to the rigidbody when input is received (units/s^2)")]
    public float MoveAcceleration = 10f;

    [Tooltip("Maximum horizontal speed (m/s). Set to <= 0 to disable clamping.")]
    public float MaxSpeed = 6f;
    
    public float JumpForce = 10f;

    Rigidbody m_Rigidbody;
    Vector2 m_MoveInput;

    private float m_JumpInput;

    private bool m_IsOnGround;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        if (m_Rigidbody == null)
            Debug.LogError("PlayerController requires a Rigidbody on the same GameObject.");
        
    }

    void OnEnable()
    {
        if (m_Asset != null && m_Asset.asset != null)
        {
            m_Asset.asset.Enable();
            m_Asset.asset.FindAction("Move").performed += HandleMove;
            m_Asset.asset.FindAction("Move").canceled  += HandleMove;
            m_Asset.asset.FindAction("Jump").performed += HandleJump;
            m_Asset.asset.FindAction("Jump").canceled  += HandleJump;
        }

        GameEventSystem.OnPlayerCollidedWithDoor += HandlePlayerCollision;
    }

    void OnDisable()
    {
        if (m_Asset != null && m_Asset.asset != null)
        {
            m_Asset.asset.FindAction("Move").performed -= HandleMove;
            m_Asset.asset.FindAction("Move").canceled  -= HandleMove;
            m_Asset.asset.FindAction("Jump").performed -= HandleJump;
            m_Asset.asset.FindAction("Jump").canceled  -= HandleJump;
            
            m_Asset.asset.Disable();
        }
        
        GameEventSystem.OnPlayerCollidedWithDoor -= HandlePlayerCollision;
    }

    void HandleMove(InputAction.CallbackContext ctx)
    {
        if (!GameManager.Instance.IsInGameplay()) return;
        
        m_MoveInput = ctx.ReadValue<Vector2>();
    }

    void HandleJump(InputAction.CallbackContext ctx)
    {
        if (!GameManager.Instance.IsInGameplay()) return;
        
        m_MoveInput.y = ctx.ReadValue<Vector2>().y;
    }

    void FixedUpdate()
    {
        if (m_Rigidbody == null)
            return;

        Vector3 desired = new Vector3(m_MoveInput.x, m_JumpInput * JumpForce, m_MoveInput.y);

        if (!m_IsOnGround) desired.y = 0f;
        
        if (desired.sqrMagnitude > 0f)
        {
            Vector3 accel = desired.normalized * MoveAcceleration;
            m_Rigidbody.AddForce(accel, ForceMode.Acceleration);
        }

        if (MaxSpeed > 0f)
        {
            Vector3 horizontalVel = new Vector3(m_Rigidbody.linearVelocity.x, 0f, m_Rigidbody.linearVelocity.z);
            float speed = horizontalVel.magnitude;
            if (speed > MaxSpeed)
            {
                Vector3 limited = horizontalVel.normalized * MaxSpeed;
                m_Rigidbody.linearVelocity = new Vector3(limited.x, m_Rigidbody.linearVelocity.y, limited.z);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            m_IsOnGround = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            m_IsOnGround = false;
        }
    }

    private void HandlePlayerCollision()
    {
        UIGameplayManager.Instance.ShowButton();
    }
}
