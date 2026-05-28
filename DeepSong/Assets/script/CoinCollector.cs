using UnityEngine;
public enum CoinRarity
{
    BRONZE,
    SILVER,
    GOLD
}
public class CoinCollector : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 100f;
    [Header("Bobbing (Mouvement vertical)")]
    [SerializeField] private float bobbingHeight = 0.5f;
    [SerializeField] private float bobbingSpeed = 2f;
    [Header("Collection")]
    [SerializeField] private int coinValue = 1;
    [SerializeField] private string playerTag = "Player";
    [Header("Effects (Optionnel)")]
    [SerializeField] private GameObject collectEffect;

    [Header("Coin Rarity")]
    private MeshFilter coinMesh;
    [SerializeField] private MeshFilter bronze;
    [SerializeField] private MeshFilter silver;
    [SerializeField] private MeshFilter gold;
    [SerializeField] private CoinRarity coinRarity;
    private Vector3 startPosition;


    [Header("Spawn Physics")]
    [SerializeField] private float launchForceMin = 3f;
    [SerializeField] private float launchForceMax = 6f;
    [SerializeField] private float upwardForce = 5f;
    [SerializeField] private LayerMask groundTag;

    private Rigidbody rb;

    private bool isLanded = false;

    public bool IsLanded
    {
        get { return isLanded; }
    }
    private float time;
    public Vector3 StartPosition
    {
        get { return startPosition; }
    }
    private void Awake()
    {
        coinMesh = GetComponent<MeshFilter>();
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        startPosition = transform.position;
        SetCoinRarity();

        if (rb != null)
        {
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            ).normalized;

            float randomForce = Random.Range(launchForceMin, launchForceMax);

           
            Vector3 launchForce = randomDirection * randomForce + Vector3.up * upwardForce;

            rb.AddForce(launchForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
        }
    }
    private void SetCoinRarity()
    {
        float random = Random.Range(0f, 100f);
        if (random < 50f)
        {
            coinRarity = CoinRarity.BRONZE;
        }
        else if (random < 80f)
        {
            coinRarity = CoinRarity.SILVER;
        }
        else
        {
            coinRarity = CoinRarity.GOLD;
        }
        switch (coinRarity)
        {
            case CoinRarity.BRONZE:
                coinMesh.sharedMesh = bronze.sharedMesh;
                coinValue = 1;
                break;
            case CoinRarity.SILVER:
                coinMesh.sharedMesh = silver.sharedMesh;
                coinValue = 5;
                break;
            case CoinRarity.GOLD:
                coinMesh.sharedMesh = gold.sharedMesh;
                coinValue = 10;
                break;
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        if (isLanded)
        {
            time += Time.deltaTime * bobbingSpeed;
            float newY = startPosition.y + Mathf.Sin(time) * bobbingHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }
    }
    public void SetStartPosition(Vector3 newPos)
    {
        startPosition = newPos;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            CollectCoin(other.gameObject);
        }
    }
    private void CollectCoin(GameObject player)
    {
        Inventory.Instance.Coins += coinValue;
        Inventory.Instance.UpdateInventory();
        AudioManager.Instance.PlaySFX("GoldPickUp", true);
        if (collectEffect != null)
        {
            GameObject coin = Instantiate(collectEffect, new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z), Quaternion.identity);
            Destroy(coin, 0.25f);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isLanded && collision.gameObject.layer == groundTag)
        {
            OnLanded();
        }
    }


    private void OnLanded()
    {
        isLanded = true;
        startPosition = transform.position;
        time = 0f;

        Debug.Log("landed");
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z), sphereCollider.radius * transform.localScale.x);
        }
    }
}