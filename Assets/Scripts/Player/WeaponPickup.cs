using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private PlayerAttack.WeaponType weaponType;

    public PlayerAttack.WeaponType WeaponType => weaponType;

    public void PickUp()
    {
        Destroy(gameObject);
    }
}
