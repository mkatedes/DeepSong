using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 12f;
    [SerializeField] private float circleRadius = 3f;
    private float circleAngle = 0f;
    float distanceToPlayer;
    [Header("LightningAttack")]
    [SerializeField] private GameObject lightingSpawnPrefab;
    [SerializeField] private GameObject lightingStrikePrefab;
    [SerializeField] private float lightningSpawnInterval = 2f;
    [SerializeField] private int numberOfLightningPerPhase = 50;
    private int currentNumberOfLightning;
    private float nextLightningTime = 0f;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int projectilesPerSalvo = 8;
    [SerializeField] private int salvoCount = 3;
    [SerializeField] private float delayBetweenSalvos = 0.3f;
    [SerializeField] private float projectileSpeed = 10f;

    [Header("Attack Pattern")]
    [SerializeField] private float timeBetweenAttackPhases = 5f;
    private enum AttackType { Lightning, Projectile, Melee }
    private AttackType currentAttack = AttackType.Lightning;
    private bool isAttacking = false;

    [Header("Component")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private CinemachineImpulseSource impulse;
    [SerializeField] private SphereCollider attackCollider;


    private GameObject player;
    private Transform playerTransform;

    private void Reset()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        CheckComponents();
    }

    private void CheckComponents()
    {
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Aucun GameObject avec le tag 'Player' trouvé !");
        }

        if (lightingSpawnPrefab == null)
        {
            Debug.LogError("lightingSpawnPrefab n'est pas assigné dans l'Inspector !");
        }

        if (lightingStrikePrefab == null)
        {
            Debug.LogError("lightingStrikePrefab n'est pas assigné dans l'Inspector !");
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("projectilePrefab n'est pas assigné dans l'Inspector !");
        }

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent n'est pas assigné !");
        }
    }

    private void Start()
    {
        StartCoroutine(DelayedStart(5));
        attackCollider.enabled = false;
    }

    private IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(AttackPatternLoop());
    }

    private void FixedUpdate()
    {
        if (playerTransform != null && agent != null)
        {
            HandleMovement();
        }
    }

    void Update()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            animator.SetBool("Invoke", isAttacking);
        }

        if (playerTransform != null)
            distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (isAttacking && currentAttack == AttackType.Lightning)
        {
            if (Time.time >= nextLightningTime && currentNumberOfLightning < numberOfLightningPerPhase)
            {
                SpawnLightningAttack();
                nextLightningTime = Time.time + lightningSpawnInterval;
            }
        }
    }

    private IEnumerator AttackPatternLoop()
    {
        while (true)
        {
            isAttacking = true;
            agent.speed = 0;

            if (currentAttack == AttackType.Lightning)
            {
                attackCollider.enabled = false;
                currentNumberOfLightning = 0;
                nextLightningTime = Time.time;

                while (currentNumberOfLightning < numberOfLightningPerPhase)
                {
                    yield return null;
                }
            }
            else if (currentAttack == AttackType.Projectile)
            {
                yield return StartCoroutine(SpawnProjectilesInSalvos());
            }
            else if(currentAttack == AttackType.Melee)
            {
                StartMeleeAttack();
            }

            isAttacking = false;
            agent.speed = 6;

            yield return new WaitForSeconds(timeBetweenAttackPhases);

            if (distanceToPlayer < 5)
            {
                currentAttack = AttackType.Melee;
            }
            else
            {
                currentAttack = (currentAttack == AttackType.Lightning)
                    ? AttackType.Projectile
                    : AttackType.Lightning;
            }
        }
    }

    private void SpawnLightningAttack()
    {
        if (player == null || lightingSpawnPrefab == null) return;

        currentNumberOfLightning++;
        GameObject spawnedWarning = Instantiate(lightingSpawnPrefab, player.transform.position, Quaternion.identity);

        LightingSpawn lightingSpawn = spawnedWarning.GetComponent<LightingSpawn>();
        if (lightingSpawn != null)
        {
            lightingSpawn.SetLightingStrikePrefab(lightingStrikePrefab);
            if(impulse != null)
                lightingSpawn.SetImpulseSource(impulse);
        }
    }

    private IEnumerator SpawnProjectilesInSalvos()
    {
        attackCollider.enabled = false;
        float rotationOffset = 0f;

        AudioManager.Instance.PlaySFX("FireStrike", true);

        for (int salvo = 0; salvo < salvoCount; salvo++)
        {
            SpawnCircleOfProjectiles(rotationOffset);
            rotationOffset += 360f / (projectilesPerSalvo * 2);

            yield return new WaitForSeconds(delayBetweenSalvos);
        }
    }


    private void SpawnCircleOfProjectiles(float offsetAngle = 0f)
    {
        float angleStep = 360f / projectilesPerSalvo;

        for (int i = 0; i < projectilesPerSalvo; i++)
        {
            float angle = (i * angleStep) + offsetAngle;

            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.transform.rotation = Quaternion.LookRotation(direction);
            projectile.GetComponent<ProjectileColliderAtk>().OwnerAttribute = GetComponent<EntityAttributes>();

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * projectileSpeed;
            }
        }
    }


    private void StartMeleeAttack()
    {
        animator.Play("BossAtkMelee");
        agent.speed = 0;
    }

    private void EndMeleeAttack()
    {
        agent.speed = 6;
    }

    public void DisableAttackCollider()
    {
         attackCollider.enabled = false;
    }

    public void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    private void HandleMovement()
    {
        if (isAttacking) return;

        Vector3 targetPosition;

        if (distanceToPlayer < minDistance)
        {
            Vector3 directionAway = (transform.position - playerTransform.position).normalized;
            targetPosition = playerTransform.position + directionAway * minDistance;
        }
        else if (distanceToPlayer > maxDistance)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            targetPosition = playerTransform.position - directionToPlayer * minDistance;
        }
        else
        {
            circleAngle += Time.deltaTime * 90f;
            if (circleAngle >= 360f) circleAngle -= 360f;

            float radians = circleAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * circleRadius;
            targetPosition = playerTransform.position + offset;
        }

        agent.SetDestination(targetPosition);

        Vector3 lookDirection = playerTransform.position - transform.position;
        lookDirection.y = 0;

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, minDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, maxDistance);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(playerTransform.position, circleRadius);
        }
    }
}