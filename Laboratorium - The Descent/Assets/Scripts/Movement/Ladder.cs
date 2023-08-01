using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private float maxHeight;

    public float GetMaxHeight()
    {
        return maxHeight;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        maxHeight = (GetComponent<SpriteRenderer>().size.y - 1.9375f) / 2f;
        Vector3 characterSize = new Vector3(1, 1.9375f, 1);

        Debug.Log((transform.position + Vector3.up * maxHeight).ToString());

        Gizmos.color = Color.red * new Color(1, 1, 1, 0.5f);
        Gizmos.DrawCube(transform.position + Vector3.up * maxHeight, characterSize);

        Gizmos.color = Color.blue * new Color(1, 1, 1, 0.5f);
        Gizmos.DrawCube(transform.position + Vector3.up * -maxHeight, characterSize);
    }
#endif
}
