using UnityEngine;
using UnityEngine.Events;

public class House3SealObject : MonoBehaviour, IInteractable
{
    [Header("봉인 번호")]
    [Range(1, 3)]
    public int sealIndex = 1;

    [Header("집3 진행 매니저")]
    public House3SequenceManager sequenceManager;

    [Header("상호작용 문구")]
    public string interactText = "<E> 봉인 해제";

    [Header("봉인 해제 후 숨길 오브젝트")]
    public GameObject sealVisualObject;

    [Header("봉인 해제 이벤트")]
    public UnityEvent onSealBroken;

    private bool broken = false;

    private void Start()
    {
        RefreshState();
    }

    private void RefreshState()
    {
        if (GameStateManager.Instance == null)
            return;

        broken = GameStateManager.Instance.IsHouse3SealBroken(sealIndex);

        if (sealVisualObject == null)
            sealVisualObject = gameObject;

        if (sealVisualObject != null)
            sealVisualObject.SetActive(!broken);
    }

    public void Interact(PlayerController player)
    {
        if (broken)
            return;

        if (GameStateManager.Instance == null)
            return;

        if (!GameStateManager.Instance.house3SecondPhaseStarted)
        {
            Debug.Log("집3: 2차 구간 시작 전이라 봉인을 해제할 수 없음");
            return;
        }

        if (sequenceManager == null)
        {
            Debug.LogWarning($"{name}: House3SequenceManager가 연결되지 않았습니다.");
            return;
        }

        broken = true;

        if (sealVisualObject != null)
            sealVisualObject.SetActive(false);

        sequenceManager.BreakSeal(sealIndex);

        onSealBroken?.Invoke();

        Debug.Log($"집3 봉인 {sealIndex} 해제");
    }

    public string GetInteractText()
    {
        if (broken)
            return "";

        return interactText;
    }
}