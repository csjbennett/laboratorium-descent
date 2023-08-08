using System;
using System.Collections.Generic;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    // Character's scripts
    private Movement charMovement;
    private Inventory charInventory;
    private Combat charCombat;

    // Ladder
    private Ladder insideLadder = null;
    private bool inLadder = false;

    // Door
    private Door insideDoor = null;
    private bool inDoor = false;

    // Item
    private Item insideKey = null;
    private bool inKey = false;

    // Weapon
    private Dictionary<int, Weapon> insideWeapons = null;
    private Weapon insideWeapon = null;
    private bool inWeapon = false;

    // Holding interact (prevents accidental multi-interacts)
    private bool holdingInteract = false;

    // Get character scripts
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    private void Start()
    {
        charMovement = GetComponent<Movement>();
        charInventory = GetComponent<Inventory>();
        charCombat = GetComponent<Combat>();

        insideWeapons = new Dictionary<int, Weapon>();
    }

    // Update is called once per frame
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    // Interaction Priorities:
    // 1: keys
    // 2: weapons
    // 3: ladders
    // 4: doors
    // 5: health packs - NOT ADDED YET!!!
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    void Update()
    {
        // Interact key
        float interact = Input.GetAxis("Interact");

        // Interact
        if (interact > 0 && !holdingInteract)
        {
            // Pick up item
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
            if (inKey)
            {
                // Pick up key
                charInventory.AddKey(insideKey.keyCode);
                Destroy(insideKey.gameObject);
                insideKey = null;
            }

            // Pick up weapon
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
            else if (inWeapon)
            {
                // If player is inside multiple weapons, then the closest one must be found
                if (insideWeapons.Count > 1)
                {
                    // Variables to help find closest weapon
                    float minDistance = 10f;
                    int closestWeapon = 0;
                    Vector2 thisPos = this.transform.position;

                    // Iterate through each weapon
                    foreach (Weapon weapon in insideWeapons.Values)
                    {
                        // Find closest weapon on ground (if there are multiple)
                        Vector2 weaponPos = weapon.transform.position;
                        float distance = Vector2.Distance(thisPos, weaponPos);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestWeapon = weapon.gameObject.GetInstanceID();
                        }

                        // Pick up closest weapon and remove from library
                        Weapon _closestWeapon;
                        insideWeapons.TryGetValue(closestWeapon, out _closestWeapon);
                        charCombat.PickupWeapon(_closestWeapon);
                        insideWeapons.Remove(closestWeapon);
                    }
                }
                // Player is only inside a single weapon - no need to find the closest one
                // Pick weapon up and remove from library of weapons
                else
                {
                    charCombat.PickupWeapon(insideWeapon);
                    insideWeapons.Remove(insideWeapon.gameObject.GetInstanceID());
                }
            }

            // Ladder mechanics
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
            else if (inLadder)
                charMovement.MountLadder(insideLadder);

            // Door mechanics
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
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

        // Prevents interact from being held and accidentally interacting with something multiple times
        if (interact == 0)
            holdingInteract = false;
    }

    // Player enters trigger
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
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
            inKey = true;
            insideKey = other.GetComponent<Item>();
        }
        else if (other.tag == "Weapon") // This is a bit more complex because the system can store multiple weapons
        {
            int id = other.gameObject.GetInstanceID();
            if (!insideWeapons.ContainsKey(id))
            {
                Weapon weapon = other.gameObject.GetComponent<Weapon>();
                insideWeapons.Add(id, weapon);
                insideWeapon = weapon;
                inWeapon = true;
            }
        }
    }

    // Player exits trigger
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
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
            inKey = false;
            insideKey = null;
        }
        else if (other.tag == "Weapon")
        {
            insideWeapons.Remove(other.gameObject.GetInstanceID());
            if (insideWeapons.Count == 0)
                inWeapon = false;
        }
    }
}
