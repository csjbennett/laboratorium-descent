using UnityEngine;

public class Interactables : MonoBehaviour
{
    // Character's scripts
    private Movement charMovement;
    private Inventory charInventory;

    // Ladder
    private Ladder insideLadder = null;
    private bool inLadder = false;

    // Door
    private Door insideDoor = null;
    private bool inDoor = false;

    // Item
    private Item insideItem = null;
    private bool inItem = false;

    // Holding interact (prevents accidental multi-interacts)
    private bool holdingInteract = false;

    // Get character scripts
    private void Start()
    {
        charMovement = GetComponent<Movement>();
        charInventory = GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        var interact = Input.GetAxis("Interact");

        // Interact
        if (interact > 0 && !holdingInteract)
        {
            // Pick up item
            if (inItem)
            {
                // Pick up key
                //if (insideItem.itemType == Item.ItemType.key)
                {
                    charInventory.AddKey(insideItem.keyCode);
                    Destroy(insideItem.gameObject);
                    insideItem = null;
                }
            }

            // Ladder mechanics
            else if (inLadder)
                charMovement.MountLadder(insideLadder);

            // Door mechanics
            else if (inDoor)
            {
                // Locked door - needs key
                if (insideDoor.IsLocked())
                {
                    // If character has key, unlock and open door
                    if (charInventory.HasKey(insideDoor.GetKeyCode()))
                    {
                        insideDoor.Unlock();
                        insideDoor.ToggleOpen();
                    }
                    // Character does not have key to open door
                    else
                        Debug.Log("locked! add message on screen later");
                }
                // Unlocked door - opens
                else
                    insideDoor.ToggleOpen();
            }

            holdingInteract = true;
        }

        if (interact == 0)
            holdingInteract = false;
    }

    // Player enters trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ladder")
        {
            inLadder = true;
            insideLadder = other.GetComponent<Ladder>();
        }
        else if (other.tag == "Door")
        {
            inDoor = true;
            insideDoor = other.GetComponent<Door>();
        }
        else if (other.tag == "Item")
        {
            inItem = true;
            insideItem = other.GetComponent<Item>();
        }
    }

    // Player exits trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ladder")
        {
            inLadder = false;
            insideLadder = null;
        }
        else if (other.tag == "Door")
        {
            inDoor = false;
            insideDoor = null;
        }
        else if (other.tag == "Item")
        {
            inItem = false;
            insideItem = null;
        }
    }
}
