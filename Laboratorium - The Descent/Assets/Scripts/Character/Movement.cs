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
    
    [Space(10)]
    [Header("Physics")]
    [SerializeField]
    private Rigidbody2D rigBod;
    [SerializeField]
    private float walkForceMultiplier;
    [SerializeField]
    private LayerMask groundLayers;

    private float xAxis = 0f;
    private float yAxis = 0f;
    private float run = 0f;
    private float interact = 0f;
    private float climb = 0f;
    private bool isGrounded = true;
    private float airtime = 0f;

    private Ladder activeLadder = null;

    public enum PlayerState { idle, moving, airborn, climbing, interacting };
    public PlayerState state;

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        xAxis = Input.GetAxis("X Axis");
        yAxis = Input.GetAxis("Y Axis");
        run = Input.GetAxis("Run");
        interact = Input.GetAxis("Interact");
        climb = Input.GetAxis("Climb");

        // Update state
        UpdateState();

        // Grounded actions
        if (state == PlayerState.idle || state == PlayerState.moving)
        {
            rigBod.drag = 1f;

            // Player movement handled by adding force to rigBod
            rigBod.AddForce(Vector2.right * GetMovementSpeed() * walkForceMultiplier * Time.deltaTime);

            // Player rotation
            if (rigBod.velocity.x < 0)
                this.transform.eulerAngles = Vector2.up * 180;
            else if (rigBod.velocity.x > 0)
                this.transform.eulerAngles = Vector2.zero;

            // Airtime
            airtime = 0f;
        }


        // Airborn actions
        else if (state == PlayerState.airborn)
        {
            rigBod.drag = 0f;

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

            if (transform.position.y > minPos && transform.position.y < maxPos)
            {
                rigBod.velocity = Vector2.up * yAxis * climbSpeed;
            }
            else if (ShouldDemount(minPos, maxPos) || climb != 0)
                DemountLadder();
        }
    }

    // OverlapArea to check if player is on the ground
    private bool IsGrounded()
    {
        Vector2 posA = (Vector2)transform.position + floorCheckA;
        Vector2 posB = (Vector2)transform.position + floorCheckB;
        return (Physics2D.OverlapArea(posA, posB, groundLayers));
    }

    private bool ShouldDemount(float minPos, float maxPos)
    {
        return (transform.position.y <= minPos && yAxis < 0) || (transform.position.y >= maxPos && yAxis > 0);
    }

    private float GetMovementSpeed()
    {
        return (xAxis * walkSpeed) + (xAxis * run * runSpeedBoost);
    }

    private void UpdateState()
    {
        if (activeLadder == null && state != PlayerState.interacting)
        {
            if (!IsGrounded())
                state = PlayerState.airborn;
            else if (xAxis != 0)
                state = PlayerState.moving;
            else
                state = PlayerState.idle;
        }
        else if (activeLadder != null)
        {
            state = PlayerState.climbing;
        }
    }

    // Mount ladder - disable gravity, change layer
    public void MountLadder(Ladder ladderToMount)
    {
        activeLadder = ladderToMount;
        transform.position = new Vector2(activeLadder.transform.position.x, transform.position.y);
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
    private void DemountLadder()
    {
        activeLadder = null;
        rigBod.gravityScale = 1f;
        rigBod.velocity = Vector2.zero;
        state = PlayerState.idle;

        gameObject.layer = LayerMask.NameToLayer("Player");
    }

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
