using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("Références")]
    private Transform player;

    [Header("Détection")]
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionAngle = 90f;

    [Header("Déplacement")]
    [SerializeField] private float normalSpeed = 3.5f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashCooldown = 2.5f;
    [SerializeField] private float dashRange = 5f;
    [SerializeField] private float circleRadius = 3f;
    [SerializeField] private float stoppingDistance = 2f;

    private NavMeshAgent agent;
    private bool playerInVision = false;
    private bool isDashing = false;
    private float lastDashTime = -10f;
    private float circleAngle = 0f;

    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider weaponCollider;

    private void Reset()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;

        agent.updateRotation = false;

        weaponCollider.enabled = false;
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        CheckPlayerInVision();

        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }

        if (playerInVision && !isDashing)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            bool canDash = Time.time >= lastDashTime + dashCooldown;

            if (distanceToPlayer <= dashRange && canDash)
            {
                StartCoroutine(DashTowardsPlayer());
            }
            else if (distanceToPlayer <= dashRange && !canDash || distanceToPlayer <= stoppingDistance)
            {
                CircleAroundPlayer();
            }
            else
            {
                agent.SetDestination(player.position);
            }
        }
        if (playerInVision)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.LookAt(player.position);
            }
        }
    }

    void CheckPlayerInVision()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;


        if (distanceToPlayer <= visionRange)
        {

            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer <= visionAngle / 2f)
            {

                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer.normalized, out hit, distanceToPlayer))
                {
                    if (hit.transform == player)
                    {
                        playerInVision = true;
                        return;
                    }
                }
            }
        }


    }

    void CircleAroundPlayer()
    {

        circleAngle += Time.deltaTime * 90f;
        if (circleAngle >= 360f) circleAngle -= 360f;


        float radians = circleAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * circleRadius;
        Vector3 circlePosition = player.position + offset;

        agent.SetDestination(circlePosition);
    }

    [SerializeField] private void EnableCollider()
    {
        weaponCollider.enabled = true;
    }

    [SerializeField] private void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
    }

    System.Collections.IEnumerator DashTowardsPlayer()
    {
        isDashing = true;
        lastDashTime = Time.time;
        if (animator != null)
        {
            animator.Play("Stab Attack", animator.GetLayerIndex("Combat"), 0f);
        }

        agent.speed = dashSpeed;
        agent.acceleration = 100f;
        agent.SetDestination(player.position);

        yield return new WaitForSeconds(dashDuration);


        agent.speed = normalSpeed;
        agent.acceleration = 8f;
        isDashing = false;
    }

    void OnDrawGizmosSelected()
    {
        // Zone de vision
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Zone de dash
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dashRange);

        // Angle de vision
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward * visionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward * visionRange;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Rayon du cercle autour du joueur
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, circleRadius);
        }
    }
}