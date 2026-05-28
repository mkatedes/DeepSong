using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Tiny;

public class CombatSystem : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private List<AttackSO> combo;
    [SerializeField] private EntityAttributes _entityAttributes;
    [SerializeField] private InputActionReference _attackAction;
    [SerializeField] private BoxCollider _hitboxCollider;
    [SerializeField] private Trail _trail;
    private float lastComboEnd;
    private int comboCounter;
    private bool isAttacking;
    private bool inputBuffered;
    private int combatLayerIndex;

    private void OnEnable()
    {
        _attackAction.action.Enable();
        _attackAction.action.performed += OnAttack;
    }

    private void OnDisable()
    {
        _attackAction.action.performed -= OnAttack;
        _attackAction.action.Disable();
    }

    private void Start()
    {
        _hitboxCollider.enabled = false;
        _trail.enabled = false;
        combatLayerIndex = _animator.GetLayerIndex("Combat");
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        if (isAttacking)
        {
            inputBuffered = true;
        }
        else
        {
            Attack();
        }
    }

    void Update()
    {
        CheckAttackState();
    }

    void Attack()
    {
        if (Time.time - lastComboEnd < 0.2f) return;
        if (comboCounter >= combo.Count) comboCounter = 0;
        AudioManager.Instance.PlaySFX("SwordSlash", true);

        _hitboxCollider.enabled = true;
        _trail.enabled = true;
        isAttacking = true;
        inputBuffered = false;

        _animator.runtimeAnimatorController = combo[comboCounter].animatorOverride;
        _animator.Play("Attack", combatLayerIndex, 0f);
        _entityAttributes.AttackDamage = (int)combo[comboCounter].damage + _entityAttributes.BaseDamage;

        comboCounter++;
    }

    void CheckAttackState()
    {
        var stateInfo = _animator.GetCurrentAnimatorStateInfo(combatLayerIndex);

        if (!stateInfo.IsTag("Attack"))
        {
            if (isAttacking)
            {
                isAttacking = false;
                Invoke(nameof(EndCombo), 0.8f);
            }
            return;
        }

        // Permet de chaîner à partir de 70% de l'animation
        if (stateInfo.normalizedTime > 0.7f && inputBuffered)
        {
            CancelInvoke(nameof(EndCombo));
            Attack();
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
        _trail.enabled = false;
        _hitboxCollider.enabled = false;
    }
}