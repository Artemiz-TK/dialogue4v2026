using UnityEngine;
using UnityEngine.InputSystem;
using Core;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private static PlayerController s_Instance;
    public static PlayerController Singleton => s_Instance;
    
    private ActionAsset m_Asset;

    [Header("Movement")]
    [Tooltip("Acceleration applied to the rigidbody when input is received (units/s^2)")]
    public float MoveAcceleration = 10f;

    [Tooltip("Maximum horizontal speed (m/s). Set to <= 0 to disable clamping.")]
    public float MaxSpeed = 6f;
    
    private float JumpForce = 7f;

    Rigidbody m_Body;
    Vector2 m_MoveInput;

    private float m_JumpInput;

    private bool m_IsOnGround;

    void Awake()
    {
        if (s_Instance != null && s_Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        s_Instance = this;
        m_Asset = new ActionAsset();
    }

    void OnEnable()
    {
        // m_Asset.Enable();
        m_Asset.asset.Enable();
        m_Asset.asset["Move"].performed += HandleMove;
        m_Asset.asset["Move"].canceled  += HandleMove;
        m_Asset.asset["Jump"].performed += HandleJump;
        m_Asset.asset["Jump"].canceled  += HandleJump;

        GameEventSystem.OnPlayerCollidedWithDoor += HandlePlayerCollision;
    }

    void OnDisable()
    {
        m_Asset.asset["Move"].performed -= HandleMove;
        m_Asset.asset["Move"].canceled  -= HandleMove;
        m_Asset.asset["Jump"].performed -= HandleJump;
        m_Asset.asset["Jump"].canceled  -= HandleJump;
        
        m_Asset.asset.Disable();
        m_Asset.Disable();
        m_Asset.Dispose();
        
        GameEventSystem.OnPlayerCollidedWithDoor -= HandlePlayerCollision;
    }

    private void Start()
    {
        m_Body = GetComponent<Rigidbody>();

        if (m_Body == null)
        {
            Debug.LogError(
                "PlayerController requires a Rigidbody on the same GameObject."
            );
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;

        if (m_Body != null)
        {
            m_Body.linearVelocity = Vector3.zero;
            m_Body.angularVelocity = Vector3.zero;
            m_Body.position = position;
        }

        Physics.SyncTransforms(); // Força a sincronização do motor de física da Unity
    }

    public async Awaitable MoveTowards(Vector3 target, float time)
    {
        var counter = 0f;
        while (counter < time)
        {
            await Awaitable.WaitForSecondsAsync(Time.deltaTime);
            m_Body.linearVelocity = new Vector3(Mathf.MoveTowards(m_Body.linearVelocity.x, target.x, (target.x - m_Body.linearVelocity.x) * counter / time),
                                                Mathf.MoveTowards(m_Body.linearVelocity.y, target.y, (target.y - m_Body.linearVelocity.y) * counter / time),
                                                Mathf.MoveTowards(m_Body.linearVelocity.z, target.z, (target.z - m_Body.linearVelocity.z) * counter / time));
            counter += Time.deltaTime;
        }
    }

    void HandleMove(InputAction.CallbackContext ctx)
    {
        if (!GameManager.Singleton.IsInGameplay()) return;
        
        m_MoveInput = ctx.ReadValue<Vector2>();
    }

    void HandleJump(InputAction.CallbackContext ctx)
    {
        if (!GameManager.Singleton.IsInGameplay()) return;
        
        if (m_IsOnGround) m_Body.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        
        m_JumpInput = ctx.ReadValue<float>();
    }

    void FixedUpdate()
    {
        if (!m_Body)
            return;

        Vector3 desired = new Vector3(m_MoveInput.x, m_JumpInput * JumpForce, m_MoveInput.y);

        if (!m_IsOnGround) desired.y = 0f;
        
        if (desired.sqrMagnitude > 0f)
        {
            Vector3 accel = desired.normalized * MoveAcceleration;
            m_Body.AddForce(accel, ForceMode.Acceleration);
        }

        if (MaxSpeed > 0f)
        {
            Vector3 horizontalVel = new Vector3(m_Body.linearVelocity.x, 0f, m_Body.linearVelocity.z);
            float speed = horizontalVel.magnitude;
            if (speed > MaxSpeed)
            {
                Vector3 limited = horizontalVel.normalized * MaxSpeed;
                m_Body.linearVelocity = new Vector3(limited.x, m_Body.linearVelocity.y, limited.z);
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
