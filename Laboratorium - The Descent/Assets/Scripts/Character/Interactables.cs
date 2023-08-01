using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    private Movement charMovement;
    private Ladder insideLadder = null;
    private bool inLadder = false;
    float interact = 0;

    private void Start()
    {
        charMovement = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inLadder && Input.GetAxis("Interact") > 0 && insideLadder != null)
            charMovement.MountLadder(insideLadder);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ladder")
        {
            inLadder = true;
            insideLadder = other.GetComponent<Ladder>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ladder")
        {
            inLadder = false;
            insideLadder = null;
        }
    }
}
