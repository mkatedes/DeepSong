using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;
using System;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine.AI;

public class EntityAttributes : MonoBehaviour
{
    [SerializeField]private int currentHealth;
    [SerializeField]private int maxHealth;
    [SerializeField]private int baseDamage;
    [SerializeField]private int armor;

    private bool isDead;
    [SerializeField]private int attackDamage;

    [SerializeField]private GameObject damageParticle;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [SerializeField]private Renderer[] entityRenderer;    
    [SerializeField]private Material material;      
    [SerializeField]private float flashDuration = 0.1f;

    private Animator animator;
    private bool immunity;

    private Material[] originalMaterials;

    [Header("Coin Spawning")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float spawnHeight = 2f; 
    [SerializeField] private float spawnRadius = 1.5f;
    [SerializeField] private float groundCheckDistance = 10f; 
    [SerializeField] private LayerMask groundLayer;

    public event Action OnPlayerDamage;

    [SerializeField]private bool hitStopActive = false;
    public int BaseDamage
    { 
        get { return baseDamage; }
        set { baseDamage = value; }
    }

    public int AttackDamage
    {
        get { return attackDamage; }
        set { attackDamage = value; }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
    }
    public int CurrentHealth
    {
        get { return currentHealth; }
    }
    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public int Armor
    {
        get { return armor; }
        set { armor = value; }
    }

    void Start()
    {
        isDead = false;
        immunity = false;
        originalMaterials = new Material[entityRenderer.Length];
        for (int i = 0; i < entityRenderer.Length; i++)
        {
            originalMaterials[i] = entityRenderer[i].material;
        }
    }

    private IEnumerator DoFlash()
    {
        foreach (var renderer in entityRenderer)
        {
            renderer.material = material;
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < entityRenderer.Length; i++)
        {
            entityRenderer[i].material = originalMaterials[i];
        }
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(DoFlash());
    }

    private void DamageFeedBack(Vector3 hitPosition)
    {
        AudioManager.Instance.PlaySFX("HitMarker");
        if(impulseSource != null)
            impulseSource?.GenerateImpulse();

        Flash();
        var particle = Instantiate(damageParticle, hitPosition, Quaternion.identity);
        Destroy(particle.gameObject, 1);
        if(hitStopActive)
            FindAnyObjectByType<HitStop>().Stop(0.05f);
    }

    public async Task TakeDamage(int amount, Vector3 hitPosition)
    {
        if (FindAnyObjectByType<Dash>().IsDashing || immunity || isDead) return;
        int damageTaken = amount - armor;
        if (damageTaken < 0)
        {
            damageTaken = 1;
        }
        currentHealth -= damageTaken;
        DamageFeedBack(hitPosition);

        OnPlayerDamage?.Invoke();
        if (gameObject.CompareTag("Player"))
        {
            PlayerDie();
        }
        else
        {
            Die();
        }

        immunity = true;
        await UniTask.WaitForSeconds(0.25f);    
        immunity = false;
    }


    public void DealDamage(GameObject target, Vector3 hitPosition)
    {
        var atm = target.GetComponent<EntityAttributes>();
        if(atm != null)
        {
            int totalDamage = attackDamage - atm.armor;

            atm.TakeDamage(totalDamage, hitPosition);
            Debug.Log("Dealt " + totalDamage + " damage to " + target.name);
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log(gameObject.name + " healed " + amount + " health. Current health: " + currentHealth);
    }

    public async Task Die()
    {
        if (isDead) return;
        if (currentHealth <= 0)
        {
            gameObject.GetComponent<NavMeshAgent>().speed = 0;
            isDead = true;
            animator.SetTrigger("Death");
            SpawnCoins(5);
            await UniTask.WaitForSeconds(1);
            Destroy(gameObject);
        }
    }

    public async Task PlayerDie()
    {
        if (isDead) return;
        if (currentHealth <= 0)
        {
            isDead = true;
            animator.SetTrigger("Death");
            AudioManager.Instance.PlaySFX("Death");
            FindAnyObjectByType<DeathCanvaCall>().ShowDeathCanva();
            Cursor.visible = true;
            await UniTask.WaitForSeconds(1);
            Destroy(gameObject);
        }
    }


    public void SpawnCoins(int amount)
    {
        if (coinPrefab == null)
        {
            Debug.LogWarning("Coin Prefab non assigné sur " + gameObject.name);
            return;
        }

        for (int i = 0; i < amount; i++)
        {
           
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, spawnHeight, randomCircle.y);

            Vector3 groundPos = FindGroundPosition(spawnPos);

            GameObject coinObj = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            CoinCollector coin = coinObj.GetComponent<CoinCollector>();

        }
    }

    private Vector3 FindGroundPosition(Vector3 fromPosition)
    {
        RaycastHit hit;

        if (Physics.Raycast(fromPosition, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            return hit.point + Vector3.up * 0.1f;
        }

        return new Vector3(fromPosition.x, transform.position.y, fromPosition.z);
    }


}
