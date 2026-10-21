using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private float interactionDistance = 3f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            TryInteract();
    }

    private void TryInteract()
    {
        if (!Physics.Raycast(
                playerCamera.transform.position,
                playerCamera.transform.forward,
                out RaycastHit hit,
                interactionDistance))
            return;

        WeaponPickup pickup = hit.collider.GetComponentInParent<WeaponPickup>();

        if (pickup != null && playerAttack != null)
        {
            if (playerAttack.AddWeapon(pickup.WeaponType))
                pickup.PickUp();

            return;
        }

        Door door = hit.collider.GetComponentInParent<Door>();

        if (door != null)
            door.Toggle();
    }
}