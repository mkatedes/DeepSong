using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenChest : MonoBehaviour, IInteractable
{
    [SerializeField] private Animation chestAnimation;
    [SerializeField] private InputActionReference _openChestAction;
    [SerializeField] private string itemName;

    

    [SerializeField]private bool randomItem = true;
    private Transform chestParent;

    private bool isOpened = false;

    private void Start()
    {
        chestParent = transform.parent;
    }

    private void Reset()
    {
        chestAnimation = GetComponentInParent<Animation>();
    }

    private void OnEnable()
    {
        _openChestAction.action.Enable();
    }

    public void Interact()
    {
        if(isOpened) return;
        OpenChestAnimation();
        if (randomItem)
            Inventory.Instance.GiveRandomItem();
        else
            Inventory.Instance.AddItemByName(itemName);

        StartCoroutine(WaitThenShrink());
    }

    private void OpenChestAnimation()
    {
        if (chestAnimation != null)
        {
            AudioManager.Instance.PlaySFX("OpenChest");

            chestAnimation.Play();
            isOpened = true;
        }
    }

    private IEnumerator ShrinkAndRotate()
    {
        float shrinkSpeed = 0.5f;
        float rotateSpeed = 180f;  
        WaitForSeconds wait = new WaitForSeconds(1f);
        while (chestParent.localScale.x > 0f)
        {
            float shrinkAmount = shrinkSpeed * Time.deltaTime;
            Vector3 newScale = chestParent.localScale - new Vector3(shrinkAmount, shrinkAmount, shrinkAmount);
            newScale = Vector3.Max(newScale, Vector3.zero);
            chestParent.localScale = newScale;

            chestParent.Rotate(0, rotateSpeed * Time.deltaTime, 0);

            yield return null;
        }
        AudioManager.Instance.PlaySFX("Pop");
        Destroy(chestParent.gameObject);
    }

    private IEnumerator WaitThenShrink()
    {
        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(ShrinkAndRotate());
    }
    public void CloseInteract()
    {
        // No action needed for closing the chest
    }
}