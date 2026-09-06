using UnityEngine;

public class ChestLid : MonoBehaviour, IInteractable
{
    [Header("회전 설정")]
    public Vector3 openRotation = new Vector3(-110f, 0f, 0f);
    public float speed = 3f;

    [Header("상호작용 문구")]
    public string openText = "<E> OPEN";
    public string closeText = "<E> CLOSE";

    private Quaternion closedRot;
    private Quaternion openRot;

    private bool isOpen = false;
    private bool isMoving = false;

    private void Start()
    {
        closedRot = transform.localRotation;
        openRot = Quaternion.Euler(openRotation) * closedRot;
    }

    private void Update()
    {
        // 핵심 최적화:
        // 상자가 움직이는 중이 아니면 회전 계산을 아예 하지 않음
        if (!isMoving)
            return;

        Quaternion targetRot = isOpen ? openRot : closedRot;

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            targetRot,
            Time.deltaTime * speed
        );

        // 목표 회전에 거의 도착하면 정확히 고정하고 Update 계산 중단
        if (Quaternion.Angle(transform.localRotation, targetRot) < 0.5f)
        {
            transform.localRotation = targetRot;
            isMoving = false;
        }
    }

    public void Interact(PlayerController player)
    {
        // 열리는 중/닫히는 중에 다시 누르면 상태가 꼬일 수 있어서 막음
        if (isMoving)
            return;

        isOpen = !isOpen;
        isMoving = true;
    }

    public string GetInteractText()
    {
        if (isMoving)
            return "...";

        return isOpen ? closeText : openText;
    }
}