using UnityEngine;

public class Ladder : MonoBehaviour
{
    private float maxHeight;

    public float GetMaxHeight()
    {
        return maxHeight;
    }

    // Character's size (for calculations)
    Vector3 characterSize = new Vector3(1, 1.9375f, 1);

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Get sprite renderer size (for calculations)
        float sizeY = GetComponent<SpriteRenderer>().size.y;

        // Set box collider height
        BoxCollider2D boxCol = GetComponent<BoxCollider2D>();
        boxCol.size = new Vector2(boxCol.size.x, sizeY);
        maxHeight = (sizeY - 1.9375f) / 2f;

        // Draw top box
        Gizmos.color = Color.red * new Color(1, 1, 1, 0.5f);
        Gizmos.DrawCube(transform.position + Vector3.up * maxHeight, characterSize);

        // Draw bottom box
        Gizmos.color = Color.blue * new Color(1, 1, 1, 0.5f);
        Gizmos.DrawCube(transform.position + Vector3.up * -maxHeight, characterSize);
    }
#endif
}
