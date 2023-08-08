using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [SerializeField]
    private Animator armAnm;
    [SerializeField]
    private Transform weaponParent;

    [SerializeField]
    private Transform armF;
    [SerializeField]
    private Transform armB;

    public bool _using = false;
    private Rigidbody2D rigBod;
    public Weapon weapon;
    Weapon.WeaponType weaponType = (Weapon.WeaponType)(-1);
    Weapon.RangedType rangedType = (Weapon.RangedType)(-1);

    private float armSpeedMod;
    public void SetArmSpeedMod(float speed)
    { armSpeedMod = speed; }

    private void Start()
    {
        rigBod = GetComponent<Rigidbody2D>();
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        weapon = newWeapon;
        weapon.combat = this;
        weaponType = weapon.GetWeaponType();
        if (weaponType == Weapon.WeaponType.ranged)
            rangedType = weapon.GetRangedType();
        weapon.transform.parent = weaponParent;
        Vector3 offsets = weapon.GetOffsets();
        weapon.transform.localPosition = new Vector2(offsets.x, offsets.y);
        weapon.transform.localEulerAngles = new Vector3(0, 0, offsets.z);
    }

    public void DropWeapon()
    {
        DisableCombat();
        weapon.ToggleRigBod(true);
        weapon.transform.parent = null;
        weapon = null;
    }

    public void DisableCombat()
    {
        weaponType = (Weapon.WeaponType)(-1);
        rangedType = (Weapon.RangedType)(-1);
    }

    private void Update()
    {
        // Weapon mechanics
        if (weapon != null)
        {
            if (Input.GetAxis("Aim") > 0)
            {
                _using = true;

                if (Input.GetAxis("Attack") > 0)
                {
                    weapon.Attack();
                }
            }
            else
                _using = false;
        }
        // Weaponless mechanics
        else
        {
            _using = false;
        }
    }
}
