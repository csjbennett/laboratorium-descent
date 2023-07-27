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
    private float jump = 0f;
    private float interact = 0f;
    private float climb = 0f;
    private bool isGrounded = true;
    private float airtime = 0f;
    private float velocity = 0f;

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
        climb = Input.GetAxis("Climb");
        run = Input.GetAxis("Run");
        interact = Input.GetAxis("Interact");

    // Update state
        UpdateState();
    }

    private void FixedUpdate()
    {
        // Grounded actions
        if (state != PlayerState.airborn)
        {
            // Player movement handled by adding force to rigBod
            rigBod.AddForce(Vector2.right * GetMovementSpeed() * walkForceMultiplier * Time.fixedDeltaTime);
            Debug.Log(rigBod.velocity.magnitude);

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
            airtime += Time.fixedDeltaTime;
        }
    }

    private bool IsGrounded()
    {
        Vector2 posA = (Vector2)transform.position + floorCheckA;
        Vector2 posB = (Vector2)transform.position + floorCheckB;
        return (Physics2D.OverlapArea(posA, posB, groundLayers));
    }

    private float GetMovementSpeed()
    {
        return (xAxis * walkSpeed) + (xAxis * run * runSpeedBoost);
    }

    private void UpdateState()
    {
        if (state != PlayerState.climbing && state != PlayerState.interacting)
        {
            if (!IsGrounded())
                state = PlayerState.airborn;
            else if (xAxis != 0)
                state = PlayerState.moving;
            else
                state = PlayerState.idle;
        }
    }

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
        Vector2[] checks = { leftCheckA, leftCheckB, rightCheckA, rightCheckB, floorCheckA, floorCheckB, headCheckA, headCheckB };

        for (int i = 0; i < checks.Length; i += 2)
        {
            Vector2 center = (Vector2)transform.position + ((checks[i] + checks[i + 1]) / 2);
            Vector2 size = new Vector2(Mathf.Abs(checks[i].x - checks[i + 1].x), Mathf.Abs(checks[i].y - checks[i + 1].y));
            Gizmos.DrawWireCube(center, size);
        }
    }
#endif
}
