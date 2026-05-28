using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private float followSpeed = 2f;

    private GameObject player;
    private Transform target;
    private Transform parentObject;
    private bool hasToFollowPlayer;

    private CoinCollector parentScript;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
        parentObject = transform.parent;
        hasToFollowPlayer = false;
        parentScript = parentObject.GetComponent<CoinCollector>();
    }

    void Update()
    {
        if (hasToFollowPlayer && parentScript.IsLanded)
            FollowPlayerAction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            hasToFollowPlayer = true;
        }
    }

    private void FollowPlayerAction()
    {
        parentScript.SetStartPosition(Vector3.MoveTowards(
            parentScript.StartPosition,
            target.position,
            followSpeed * Time.deltaTime
        ));
    }
}
