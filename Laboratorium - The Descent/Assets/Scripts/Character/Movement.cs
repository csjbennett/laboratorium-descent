using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeedBoost;
    [SerializeField]
    private float climbSpeed;

    [SerializeField]
    private float walkAnmSpeedMod = 1f;
    
    [Space(10)]
    [Header("Physics")]
    [SerializeField]
    private Rigidbody2D rigBod;
    [SerializeField]
    private float walkForceMultiplier;
    [SerializeField]
    private LayerMask groundLayers;
    [SerializeField]
    private float airDrag;

    private float xAxis = 0f;
    private float yAxis = 0f;
    private float run = 0f;
    private float interact = 0f;
    private float climb = 0f;
    private bool isGrounded = true;
    private bool waitingLayerChange = false;
    private float airtime = 0f;

    private Ladder activeLadder = null;
    private Combat combat;

    public enum PlayerState { idle, moving, airborn, climbing, interacting };
    public PlayerState state;

    private void Start()
    {
        combat = GetComponent<Combat>();
        combat.SetArmSpeedMod(walkAnmSpeedMod);
    }

    // Update is called once per frame
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    void Update()
    {
        // Inputs
        xAxis = Input.GetAxis("X Axis");
        yAxis = Input.GetAxis("Y Axis");
        run = Input.GetAxis("Run");
        interact = Input.GetAxis("Interact");
        climb = Input.GetAxis("Climb");

        // Airborn actions
        if (state == PlayerState.airborn)
        {
            rigBod.drag = airDrag;

            // Increase airtime if player is falling
            if (rigBod.velocity.y < 0)
                airtime += Time.deltaTime;
        }
        // Climbing
        else if (state == PlayerState.climbing)
        {
            float max = activeLadder.GetMaxHeight();
            float minPos = activeLadder.transform.position.y - max;
            float maxPos = activeLadder.transform.position.y + max;
            Debug.Log(maxPos.ToString() + ", " + minPos.ToString());

            // Keep player within ladder bounds
            if (transform.position.y > minPos && transform.position.y < maxPos)
            {
                // Lerp player to ladder
                if (transform.position.x != activeLadder.transform.position.x)
                {
                    float xTarget = Mathf.Lerp(transform.position.x, activeLadder.transform.position.x, Time.deltaTime * 10f);
                    transform.position = new Vector2(xTarget, transform.position.y + (yAxis * climbSpeed * Time.deltaTime));
                }

                // Move up and down ladder
                rigBod.velocity = Vector2.up * yAxis * climbSpeed;
            }
            // Check if player should demount ladder
            else if (ShouldDemount(minPos, maxPos))
                DemountLadder(true);
        }
        // Grounded (reset airtime)
        else
            airtime = 0f;

        // Update state
        UpdateState();
    }

    // FixedUpdate for ground movement to prevent jittering
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    private void FixedUpdate()
    {
        // Grounded actions
        if (state == PlayerState.idle || state == PlayerState.moving)
        {
            rigBod.drag = 1f;

            // Player movement handled by adding force to rigBod
            rigBod.AddForce(Vector2.right * GetMovementSpeed() * walkForceMultiplier * Time.fixedDeltaTime);

            // Player rotation
            if (rigBod.velocity.x < 0)
                this.transform.eulerAngles = Vector2.up * 180;
            else if (rigBod.velocity.x > 0)
                this.transform.eulerAngles = Vector2.zero;
        }
    }

    // OverlapArea to check if player is on the ground, or if there is an object to the right or left of them
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    private bool IsGrounded()
    {
        Vector2 posA = (Vector2)transform.position + floorCheckA;
        Vector2 posB = (Vector2)transform.position + floorCheckB;
        return Physics2D.OverlapArea(posA, posB, groundLayers);
    
    }
    private bool WallToTheRight()
    {
        Vector2 posA = (Vector2)transform.position + rightCheckA;
        Vector2 posB = (Vector2)transform.position + rightCheckB;
        return Physics2D.OverlapArea(posA, posB, groundLayers);
    }
    private bool WallToTheLeft()
    {
        Vector2 posA = (Vector2)transform.position + leftCheckA;
        Vector2 posB = (Vector2)transform.position + leftCheckB;
        return Physics2D.OverlapArea(posA, posB, groundLayers);
    }


    // Determines whether player is at top or bottom of ladder
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    private bool ShouldDemount(float minPos, float maxPos)
    {
        return (transform.position.y <= minPos && yAxis < 0) || (transform.position.y >= maxPos && yAxis > 0);
    }


    // Gets player's movement inputs
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    private float GetMovementSpeed()
    {
        float weaponMod = 1f;
        float runMod = (xAxis * run * runSpeedBoost);
        // Slow player down when weapon is in use
        if (combat._using)
        {
            weaponMod = 0.5f;
            runMod = 0;
        }
        return (xAxis * walkSpeed * weaponMod) + runMod;
    }


    // Player state machine
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    private void UpdateState()
    {
        if (activeLadder == null && state != PlayerState.interacting)
        {
            if (!IsGrounded())
                state = PlayerState.airborn;
            else
            {
                if (xAxis != 0)
                    state = PlayerState.moving;
                else
                    state = PlayerState.idle;

                if (waitingLayerChange)
                {
                    gameObject.layer = LayerMask.NameToLayer("Player");
                    waitingLayerChange = false;
                }
            }
        }
        else if (activeLadder != null)
        {
            // Jank workaround (I didn't want
            // to have any inputs be handled
            // here, but this was the only way
            // to get manual demounting working)

            if (climb == 0)
                state = PlayerState.climbing;
            else
                DemountLadder(false);
        }
    }


    // Mount ladder - disable gravity, change layer
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    public void MountLadder(Ladder ladderToMount)
    {
        activeLadder = ladderToMount;
        transform.rotation = Quaternion.identity;
        rigBod.gravityScale = 0f;

        gameObject.layer = LayerMask.NameToLayer("PlayerLadder");

        // Move player within ladder bounds
        float min = activeLadder.transform.position.y - activeLadder.GetMaxHeight();
        float max = activeLadder.transform.position.y + activeLadder.GetMaxHeight();
        if (transform.position.y < min)
            transform.position = new Vector3(transform.position.x, min);
        else if (transform.position.y > max)
            transform.position = new Vector3(transform.position.x, max);
    }

    // Demount ladder - enable gravity, change layer
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    private void DemountLadder(bool extents)
    {
        activeLadder = null;
        rigBod.gravityScale = 1f;
        rigBod.velocity = Vector2.zero;
        UpdateState();

        if (extents)
            gameObject.layer = LayerMask.NameToLayer("Player");
        else
            waitingLayerChange = true;
    }

    // Manually update player's state
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    public void UpdateState(PlayerState newState)
    {
        state = newState;
    }



    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    // ~~~ Area Checks ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //

    [Space(10)]
    [Header("Area checks")]
    [SerializeField]
    private bool drawGizmos = true;
    [SerializeField]
    private Vector2 leftCheckA;
    [SerializeField]
    private Vector2 leftCheckB;
    [SerializeField]
    private Vector2 rightCheckA;
    [SerializeField]
    private Vector2 rightCheckB;
    [SerializeField]
    private Vector2 floorCheckA;
    [SerializeField]
    private Vector2 floorCheckB;
    [SerializeField]
    private Vector2 headCheckA;
    [SerializeField]
    private Vector2 headCheckB;

#if UNITY_EDITOR
    // Gizmos for area checks
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Vector2[] checks = { leftCheckA, leftCheckB, rightCheckA, rightCheckB, floorCheckA, floorCheckB, headCheckA, headCheckB };

            for (int i = 0; i < checks.Length; i += 2)
            {
                Vector2 center = (Vector2)transform.position + ((checks[i] + checks[i + 1]) / 2);
                Vector2 size = new Vector2(Mathf.Abs(checks[i].x - checks[i + 1].x), Mathf.Abs(checks[i].y - checks[i + 1].y));
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
#endif
}
