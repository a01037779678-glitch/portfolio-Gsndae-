using UnityEngine;
using UnityEngine.Events;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("월드 아이템 고유 ID")]
    public string worldItemId;

    [Header("획득 아이템 정보")]
    public string itemId = "PinkTotem";
    public string itemName = "분홍여자토템";

    [Header("중첩 설정")]
    public bool stackable = false;
    public int maxStack = 1;

    [Header("상호작용 문구")]
    public string interactText = "<E> 획득";

    [Header("획득 성공 시 이벤트")]
    public UnityEvent onPickedUp;

    [Header("획득 시 활성화할 오브젝트")]
    public GameObject activateObject;

    [Header("토템 획득 후 잠글 문")]
    public SlidingTrapDoor trapDoor;

    private void Start()
    {
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsWorldItemPicked(worldItemId))
        {
            gameObject.SetActive(false);
        }
    }

    public void Interact(PlayerController player)
    {
        if (player == null)
            return;

        // ItemSwitcher가 Player 본체가 아니라 HandItems 같은 자식에 있어도 찾게 함
        ItemSwitcher itemSwitcher = player.GetComponentInChildren<ItemSwitcher>(true);

        if (itemSwitcher == null)
        {
            Debug.LogWarning($"{name}: Player 또는 Player 자식에서 ItemSwitcher를 찾지 못했습니다.");
            return;
        }

        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning($"{name}: Item Id가 비어 있습니다.");
            return;
        }

        GameObject handItemObject = itemSwitcher.GetHandItemObject(itemId);
        GameObject worldPickupPrefab = itemSwitcher.GetWorldPickupPrefab(itemId);

        if (handItemObject == null)
        {
            Debug.LogWarning($"{name}: Item Id [{itemId}]에 맞는 Hand Item Object를 찾지 못했습니다. Player의 ItemSwitcher > Item Definitions를 확인하세요.");
            return;
        }

        bool added = itemSwitcher.AddItem(
            itemId,
            itemName,
            handItemObject,
            worldPickupPrefab,
            stackable,
            maxStack
        );

        // 인벤토리가 꽉 찼거나 추가 실패하면 아이템을 없애면 안 됨
        if (!added)
            return;

        if (activateObject != null)
            activateObject.SetActive(true);

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.SetWorldItemPicked(worldItemId);

        onPickedUp?.Invoke();

        if (trapDoor != null)
        {
            trapDoor.LockAndCloseDoor();
        }

        Destroy(gameObject);
    }

    public string GetInteractText()
    {
        return interactText;
    }
}