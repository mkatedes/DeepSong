using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Input")]
    [SerializeField] private InputActionReference dashAction;

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private TrailRenderer dashTrail;

    private float lastDashTime = -Mathf.Infinity;
    private bool isDashing = false;
    private float dashEndTime = 0f;
    [SerializeField]private Rigidbody rb;

    public bool IsDashing => isDashing;
    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        dashTrail = GetComponentInChildren<TrailRenderer>();
        animator = GetComponent<Animator>();

        dashTrail.enabled = false;
    }

    void OnEnable()
    {
        dashAction.action.Enable();
    }

    void OnDisable()
    {
        dashAction.action.Disable();
    }

    void Update()
    {
        if (dashAction.action.triggered && Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            StartDash();
        }

        if (isDashing && Time.time >= dashEndTime)
        {
            EndDash();
        }
    }

    private void StartDash()
    {
        Vector3 dashDirection = transform.forward;
        dashDirection.y = 0f;
        dashDirection.Normalize();
        
        AudioManager.Instance.PlaySFX("Dodge", true);
        animator.Play("Dodge_Forward", 0);
        animator.Play("Dodge_Forward", animator.GetLayerIndex("Combat"), 0);
        dashTrail.enabled = true;

        isDashing = true;
        dashEndTime = Time.time + dashDuration;
        lastDashTime = Time.time;

        rb.linearVelocity = dashDirection * dashSpeed;
    }

 

    private void EndDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector3.zero;
        dashTrail.enabled = false;
    }
}