using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class Interact : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;
    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera interactionCam;

    [SerializeField] private Transform playerCameraRoot;
    private bool interaction = false;
    private GameObject player;
    private GameObject currentInteractable = null;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }
    private void OnEnable()
    {
        interactAction.action.Enable();
        interactAction.action.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteractPerformed;
        interactAction.action.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            currentInteractable = other.gameObject.GetComponent<IInteractable>() != null ? other.gameObject : null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentInteractable)
        {
            if (interaction)
            {
                EndInteraction(currentInteractable.GetComponent<IInteractable>());
            }
            currentInteractable = null;
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (currentInteractable == null)
            return;

        IInteractable interactableTarget = currentInteractable.GetComponent<IInteractable>();

        if (interactableTarget == null)
            return;

        if (!interaction)
        {
            interactableTarget.Interact();
            StartInteractionCinematic(currentInteractable);

            interaction = true;
            player.GetComponent<PlayerMovement>().enabled = false;
            player.GetComponent<CombatSystem>().enabled = false;

            if (interactableTarget is OpenChest)
            {
                StartCoroutine(WaitThenStopInteractionCinematic(interactableTarget));
            }
        }
        else
        {
            EndInteraction(interactableTarget);
        }
    }

    private IEnumerator WaitThenStopInteractionCinematic(IInteractable interactableTarget)
    {
        yield return new WaitForSeconds(4.2f);

        if (interaction)
        {
            EndInteraction(interactableTarget);
        }
    }

    private void EndInteraction(IInteractable interactableTarget)
    {
        interactableTarget.CloseInteract();
        StopInteractionCinematic();
        interaction = false;
        player.GetComponent<PlayerMovement>().enabled = true;
        player.GetComponent<CombatSystem>().enabled = true;
    }

    private void StartInteractionCinematic(GameObject targetObject)
    {
        if (interactionCam == null || playerCameraRoot == null) return;

        interactionCam.Follow = playerCameraRoot;
        interactionCam.LookAt = targetObject.transform;
        interactionCam.Priority = 20;
    }

    public void StopInteractionCinematic()
    {
        if (interactionCam == null) return;

        interactionCam.Priority = 9;
        interactionCam.Follow = null;
        interactionCam.LookAt = null;
    }


}


