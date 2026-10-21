using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public enum WeaponType
    {
        Knife,
        Pistol,
        SniperRifle,
        AssaultRifle
    }

    [System.Serializable]
    public class WeaponData
    {
        public WeaponType type;
        public float range = 20f;
        public float damage = 15f;
        public GameObject handPrefab;
    }

    [Header("Settings")]
    [SerializeField] private WeaponData[] weaponDefinitions;
    [SerializeField] private Transform handSlot;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float knifeRange = 1.5f;

    private readonly List<WeaponData> inventory = new();
    private GameObject heldWeapon;
    private int currentWeaponIndex;

    private void Update()
    {
        if (inventory.Count == 0) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
            currentWeaponIndex = (currentWeaponIndex + 1) % inventory.Count;
        else if (scroll < 0f)
            currentWeaponIndex = (currentWeaponIndex - 1 + inventory.Count) % inventory.Count;

        if (scroll != 0f)
            ShowCurrentWeapon();

        if (Input.GetMouseButtonDown(0))
            Attack();
    }

    public bool HasWeapon(WeaponType type)
    {
        return inventory.Exists(weapon => weapon.type == type);
    }

    public bool AddWeapon(WeaponType type)
    {
        if (HasWeapon(type)) return false;

        foreach (WeaponData definition in weaponDefinitions)
        {
            if (definition.type != type) continue;

            inventory.Add(definition);
            currentWeaponIndex = inventory.Count - 1;
            ShowCurrentWeapon();
            return true;
        }

        Debug.LogWarning($"No weapon settings: {type}");
        return false;
    }

    private void ShowCurrentWeapon()
    {
        if (heldWeapon != null)
            Destroy(heldWeapon);

        WeaponData weapon = inventory[currentWeaponIndex];

        if (handSlot == null || weapon.handPrefab == null)
        {
            Debug.LogWarning($"Define Weapon Slot and Prefab of a weapon for {weapon.type}");
            return;
        }

        heldWeapon = Instantiate(weapon.handPrefab, handSlot);
        heldWeapon.transform.localPosition = Vector3.zero;
        heldWeapon.transform.localRotation = Quaternion.identity;
        heldWeapon.transform.localScale = Vector3.one;
    }

    private void Attack()
    {
        WeaponData weapon = inventory[currentWeaponIndex];

        if (weapon.type == WeaponType.Knife)
        {
            if (Physics.SphereCast(
                playerCamera.transform.position,
                0.25f,
                playerCamera.transform.forward,
                out RaycastHit hit,
                knifeRange,
                enemyLayers))
            {
                DamageEnemy(hit.collider, weapon.damage);
            }

            return;
        }

        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out RaycastHit shot,
            weapon.range,
            enemyLayers))
        {
            DamageEnemy(shot.collider, weapon.damage);
        }
    }

    private void DamageEnemy(Collider hitCollider, float damage)
    {
        EnemyHealth enemy = hitCollider.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
            enemy.TakeDamage(damage);
    }
}