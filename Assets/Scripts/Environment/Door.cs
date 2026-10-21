using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float animationSpeed = 5f;

    private bool isOpen;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        if (doorPivot == null)
            doorPivot = transform;

        closedRotation = doorPivot.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    private void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        doorPivot.localRotation = Quaternion.Slerp(
            doorPivot.localRotation,
            targetRotation,
            animationSpeed * Time.deltaTime
        );
    }

    public void Toggle()
    {
        isOpen = !isOpen;
    }
}