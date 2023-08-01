using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float cameraEaseSpeed;
    [SerializeField]
    private float rigBodInfluence;
    [SerializeField][Tooltip("The amount of influence the player's position has on the camera position")]
    private int playerBias;
    [SerializeField][Tooltip("The amount of influence the mouse's position has on the camera position")]
    private int mouseBias;

    private Rigidbody2D rigBod;
    private Transform player;
    private Camera _cam;
    private Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        _cam = Camera.main;
        cam = _cam.transform;
        player = this.transform;
        rigBod = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
    // Current position
        Vector3 currentPos = cam.position;

    // Mouse position
        Vector2 mousePos = Input.mousePosition;
        float screenMaxX = Screen.currentResolution.width;
        float screenMaxY = Screen.currentResolution.height;
        mousePos = _cam.ScreenToWorldPoint(new Vector2(Mathf.Clamp(mousePos.x, 0, screenMaxX), Mathf.Clamp(mousePos.y, 0, screenMaxY)));

    // Player position with velocity offset (moving in a direction will push the camera out that way)
        Vector3 playerPos = player.position;
        float xInfluence = rigBod.velocity.x * rigBodInfluence;
        float yInfluence = rigBod.velocity.y * rigBodInfluence;
        Vector3 playerPosWithOffset = new Vector3(playerPos.x + xInfluence, playerPos.y + yInfluence, 0);

    // Target position
        Vector3 targetPos = ((playerPosWithOffset * playerBias) + ((Vector3)mousePos * mouseBias)) / (playerBias + mouseBias) + (Vector3.back * 10f);

        cam.position = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * cameraEaseSpeed);
    }
}
