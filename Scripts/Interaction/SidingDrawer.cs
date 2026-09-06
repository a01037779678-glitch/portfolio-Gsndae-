using UnityEngine;

public class SlidingDrawer : MonoBehaviour, IInteractable
{
    [Header("서랍 이동 설정 - 로컬 기준")]
    public Vector3 openLocalOffset = new Vector3(0f, 0f, 0.3f);
    public float speed = 2f;

    [Header("열기 전 플레이어 막힘 검사")]
    public bool blockIfPlayerInFront = true;

    [Tooltip("플레이어가 있으면 열리지 않게 검사할 박스 중심. 비워두면 서랍 위치 기준")]
    public Transform blockCheckCenter;

    [Tooltip("검사 박스 크기. 서랍이 앞으로 나오는 공간만큼 잡기")]
    public Vector3 blockCheckHalfExtents = new Vector3(0.4f, 0.8f, 0.35f);

    [Tooltip("Player 레이어를 넣는 걸 추천")]
    public LayerMask playerLayer;

    [Header("잠금 설정")]
    public bool isLocked = false;
    public string requiredKeyId = "AKey";

    [Header("상호작용 문구")]
    public string lockedText = "<E> LOCKED";
    public string openText = "<E> OPEN";
    public string closeText = "<E> CLOSE";
    public string blockedText = "조금 뒤로 물러나세요";

    private Vector3 closedLocalPosition;
    private Vector3 openLocalPosition;

    private bool isOpen = false;
    private bool isMoving = false;
    private bool isBlocked = false;

    private void Start()
    {
        closedLocalPosition = transform.localPosition;
        openLocalPosition = closedLocalPosition + openLocalOffset;
    }

    private void Update()
    {
        if (!isMoving)
            return;

        Vector3 targetPosition = isOpen ? openLocalPosition : closedLocalPosition;

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            targetPosition,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.localPosition, targetPosition) < 0.001f)
        {
            transform.localPosition = targetPosition;
            isMoving = false;
        }
    }

    public void Interact(PlayerController player)
    {
        if (isMoving)
            return;

        if (isLocked)
        {
            if (player == null || !player.HasKey(requiredKeyId))
            {
                Debug.Log("NEED KEY");
                return;
            }

            isLocked = false;
            Debug.Log("DRAWER UNLOCKED");
        }

        // 닫힌 상태에서 열려고 할 때만 플레이어 위치 검사
        if (!isOpen && blockIfPlayerInFront && IsPlayerBlockingDrawer())
        {
            isBlocked = true;
            Debug.Log($"{name}: 플레이어가 너무 가까워서 서랍을 열 수 없습니다.");
            return;
        }

        isBlocked = false;
        isOpen = !isOpen;
        isMoving = true;
    }

    public string GetInteractText()
    {
        if (isLocked)
            return lockedText;

        if (isMoving)
            return "...";

        if (isBlocked)
            return blockedText;

        return isOpen ? closeText : openText;
    }

    private bool IsPlayerBlockingDrawer()
    {
        Vector3 center = blockCheckCenter != null
            ? blockCheckCenter.position
            : transform.position + transform.TransformDirection(openLocalOffset * 0.5f);

        Quaternion rotation = transform.rotation;

        Collider[] hits = Physics.OverlapBox(
            center,
            blockCheckHalfExtents,
            rotation,
            playerLayer,
            QueryTriggerInteraction.Ignore
        );

        return hits.Length > 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (!blockIfPlayerInFront)
            return;

        Vector3 center = blockCheckCenter != null
            ? blockCheckCenter.position
            : transform.position + transform.TransformDirection(openLocalOffset * 0.5f);

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, blockCheckHalfExtents * 2f);
    }
}