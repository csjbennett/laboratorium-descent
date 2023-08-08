using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType { melee, ranged, throwable };
    [SerializeField]
    private WeaponType weaponType;

    // Basic attributes
    [SerializeField][Tooltip("Damage per hit")]
    private float damage;
    [SerializeField][Tooltip("Delay between attacks")]
    private float attackDelay;
    [SerializeField]
    private Vector2 weaponOffset;
    [SerializeField]
    private float weaponRotation;

    // Gun-specific attributes
    public enum RangedType { oneHanded, twoHanded };            // Ranged weapon types
    [SerializeField][DrawIf("weaponType", WeaponType.ranged)]
    private RangedType rangedType;
    [SerializeField][DrawIf("weaponType", WeaponType.ranged)]
    private GameObject bullet;                                  // Bullet prefab
    [SerializeField][DrawIf("weaponType", WeaponType.ranged)]
    private int magSize;                                        // How many bullets the gun holds

    // Throwable-specific attributes
    [SerializeField][DrawIf("WeaponType", WeaponType.throwable)]
    private GameObject throwable;

    // Combat script
    public Combat combat = null;

    // Physics components
    private Rigidbody2D rigBod;
    private BoxCollider2D boxCol;

    // Weapon type getter
    public WeaponType GetWeaponType()
    { return weaponType; }
    public RangedType GetRangedType()
    { return rangedType; }
    public Vector3 GetOffsets()
    { return new Vector3(weaponOffset.x, weaponOffset.y, weaponRotation); }
    public int GetAmmo()
    { return magSize; }


    // Attack
    public void Attack()
    {

    }

    public void ToggleRigBod(bool enabled)
    {
        rigBod.simulated = enabled;
        boxCol.enabled = enabled;
    }

    IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(attackDelay);
        
    }
}
