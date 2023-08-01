using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractableType { ladder, door, pickup };
    [SerializeField]
    private InteractableType interactableType;

    [DrawIf("interactableType", InteractableType.pickup)][SerializeField]
    private string itemId;

    private bool canInteract = false;

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.tag == "Player")
            canInteract = true;
    }

    private void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D.tag == "Player")
            canInteract = false;
    }

    private void Update()
    {
        if (canInteract)
        {
            if (Input.GetAxis("Interact") != 0)
            {
                Interact();
            }
        }
    }

    private void Interact()
    {

    }
}
