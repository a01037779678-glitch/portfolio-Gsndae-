using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class ItemSlot
    {
        public string itemId;
        public string itemName;
        public GameObject itemObject;
        public bool unlocked;

        [Header("드롭 설정")]
        public GameObject worldPickupPrefab;

        [Header("중첩 설정")]
        public bool stackable;
        public int count;
        public int maxStack;
    }

    [System.Serializable]
    public class ItemDefinition
    {
        public string itemId;
        public GameObject handItemObject;
        public GameObject worldPickupPrefab;
    }

    [Header("아이템 정의")]
    public ItemDefinition[] itemDefinitions;

    [Header("슬롯 설정")]
    public ItemSlot[] slots = new ItemSlot[4];

    [Header("현재 선택 슬롯")]
    public int currentIndex = 0;

    [Header("드롭 설정")]
    public Transform dropPoint;
    public float dropForce = 2f;

    private void Start()
    {
        UpdateItemVisibility();
    }

    private void Update()
    {
        HandleNumberKeys();
        HandleMouseWheel();

        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            DropCurrentItem();
        }
    }

    private void HandleNumberKeys()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) TrySelectSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) TrySelectSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) TrySelectSlot(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) TrySelectSlot(3);
    }

    private void HandleMouseWheel()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll > 0f) SelectPrevious();
        else if (scroll < 0f) SelectNext();
    }

    public GameObject GetHandItemObject(string itemId)
    {
        if (itemDefinitions == null) return null;

        for (int i = 0; i < itemDefinitions.Length; i++)
        {
            if (itemDefinitions[i] != null && itemDefinitions[i].itemId == itemId)
                return itemDefinitions[i].handItemObject;
        }

        return null;
    }

    public GameObject GetWorldPickupPrefab(string itemId)
    {
        if (itemDefinitions == null) return null;

        for (int i = 0; i < itemDefinitions.Length; i++)
        {
            if (itemDefinitions[i] != null && itemDefinitions[i].itemId == itemId)
                return itemDefinitions[i].worldPickupPrefab;
        }

        return null;
    }

    public bool AddItem(
        string newItemId,
        string newItemName,
        GameObject newItemObject,
        GameObject newWorldPickupPrefab,
        bool stackable,
        int maxStack = 99
    )
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].unlocked) continue;

            if (slots[i].itemId == newItemId)
            {
                if (!slots[i].stackable)
                {
                    Debug.Log($"{newItemName}은(는) 하나만 소지 가능합니다.");
                    return false;
                }

                if (slots[i].count >= slots[i].maxStack)
                {
                    Debug.Log($"{newItemName}의 최대 개수에 도달했습니다.");
                    return false;
                }

                slots[i].count++;
                currentIndex = i;

                Debug.Log($"{newItemName} 중첩 획득: {slots[i].count}");

                UpdateItemVisibility();
                return true;
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].unlocked)
            {
                slots[i].itemId = newItemId;
                slots[i].itemName = newItemName;
                slots[i].itemObject = newItemObject;
                slots[i].worldPickupPrefab = newWorldPickupPrefab;
                slots[i].unlocked = true;
                slots[i].stackable = stackable;
                slots[i].count = 1;
                slots[i].maxStack = maxStack;

                currentIndex = i;

                Debug.Log($"아이템 추가 완료: {newItemName} -> 슬롯 {i + 1}");

                UpdateItemVisibility();
                return true;
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다.");
        return false;
    }

    public bool HasItem(string itemId)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].unlocked && slots[i].itemId == itemId)
                return true;
        }

        return false;
    }

    public bool RemoveItem(string itemId)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].unlocked)
                continue;

            if (slots[i].itemId == itemId)
            {
                if (slots[i].stackable && slots[i].count > 1)
                {
                    slots[i].count--;
                }
                else
                {
                    if (slots[i].itemObject != null)
                        slots[i].itemObject.SetActive(false);

                    ClearSlot(i);

                    if (GetUnlockedCount() > 0)
                        SelectNextAvailableSlot();
                    else
                        currentIndex = 0;
                }

                UpdateItemVisibility();
                return true;
            }
        }

        return false;
    }

    private void TrySelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        if (!slots[index].unlocked) return;

        currentIndex = index;
        UpdateItemVisibility();
    }

    private void SelectNext()
    {
        if (GetUnlockedCount() == 0) return;

        int startIndex = currentIndex;

        do
        {
            currentIndex = (currentIndex + 1) % slots.Length;

            if (slots[currentIndex].unlocked)
            {
                UpdateItemVisibility();
                return;
            }
        }
        while (currentIndex != startIndex);
    }

    private void SelectPrevious()
    {
        if (GetUnlockedCount() == 0) return;

        int startIndex = currentIndex;

        do
        {
            currentIndex--;

            if (currentIndex < 0)
                currentIndex = slots.Length - 1;

            if (slots[currentIndex].unlocked)
            {
                UpdateItemVisibility();
                return;
            }
        }
        while (currentIndex != startIndex);
    }

    private int GetUnlockedCount()
    {
        int count = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].unlocked)
                count++;
        }

        return count;
    }

    public void DropCurrentItem()
    {
        if (currentIndex < 0 || currentIndex >= slots.Length) return;
        if (!slots[currentIndex].unlocked) return;

        var slot = slots[currentIndex];

        if (slot.itemObject != null)
            slot.itemObject.SetActive(false);

        if (slot.worldPickupPrefab != null && dropPoint != null)
        {
            GameObject dropped = Instantiate(
                slot.worldPickupPrefab,
                dropPoint.position,
                Quaternion.identity
            );

            Rigidbody rb = dropped.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(dropPoint.forward * dropForce, ForceMode.Impulse);
            }
        }

        ClearSlot(currentIndex);

        if (GetUnlockedCount() > 0)
            SelectNextAvailableSlot();
        else
            UpdateItemVisibility();
    }

    private void ClearSlot(int index)
    {
        slots[index].itemId = "";
        slots[index].itemName = "";
        slots[index].itemObject = null;
        slots[index].worldPickupPrefab = null;
        slots[index].unlocked = false;
        slots[index].stackable = false;
        slots[index].count = 0;
        slots[index].maxStack = 0;
    }

    private void SelectNextAvailableSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].unlocked)
            {
                currentIndex = i;
                UpdateItemVisibility();
                return;
            }
        }
    }

    private void OnEnable()
    {
        UpdateItemVisibility();
    }
    public void UpdateItemVisibility()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemObject == null)
                continue;

            bool shouldShow = slots[i].unlocked && i == currentIndex;
            slots[i].itemObject.SetActive(shouldShow);
        }
    }

    public string GetCurrentItemId()
    {
        if (currentIndex < 0 || currentIndex >= slots.Length)
            return "";

        if (!slots[currentIndex].unlocked)
            return "";

        return slots[currentIndex].itemId;
    }

    public string GetCurrentItemName()
    {
        if (currentIndex < 0 || currentIndex >= slots.Length)
            return "";

        if (!slots[currentIndex].unlocked)
            return "";

        return slots[currentIndex].itemName;
    }

    public void RestoreHeldItem(string itemId, string itemName)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            ClearAllSlots();
            UpdateItemVisibility();
            return;
        }

        ClearAllSlots();


        GameObject handObject = GetHandItemObject(itemId);
        GameObject worldPrefab = GetWorldPickupPrefab(itemId);

        AddItem(
            itemId,
            itemName,
            handObject,
            worldPrefab,
            false,
            1
        );

        UpdateItemVisibility();

        Debug.Log($"손 아이템 복구 완료: {itemId}");
    }

    public void ClearAllSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemObject != null)
                slots[i].itemObject.SetActive(false);

            slots[i].itemId = "";
            slots[i].itemName = "";
            slots[i].itemObject = null;
            slots[i].worldPickupPrefab = null;
            slots[i].unlocked = false;
            slots[i].stackable = false;
            slots[i].count = 0;
            slots[i].maxStack = 0;
        }
        currentIndex = 0;
    }

    public SavedItemSlot[] GetSavedSlots()
    {
        SavedItemSlot[] savedSlots = new SavedItemSlot[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            savedSlots[i] = new SavedItemSlot();

            savedSlots[i].itemId = slots[i].itemId;
            savedSlots[i].itemName = slots[i].itemName;
            savedSlots[i].unlocked = slots[i].unlocked;

            savedSlots[i].stackable = slots[i].stackable;
            savedSlots[i].count = slots[i].count;
            savedSlots[i].maxStack = slots[i].maxStack;
        }

        return savedSlots;
    }

    public void RestoreSlots(SavedItemSlot[] savedSlots, int savedCurrentIndex)
    {
        // 먼저 현재 인벤토리 제거
        ClearAllSlots();

        if (savedSlots == null || savedSlots.Length == 0)
        {
            UpdateItemVisibility();
            Debug.LogWarning("복구할 인벤토리 슬롯 데이터가 없습니다.");
            return;
        }

        int length = Mathf.Min(savedSlots.Length, slots.Length);

        for (int i = 0; i < length; i++)
        {
            if (savedSlots[i] == null || !savedSlots[i].unlocked)
                continue;

            string itemId = savedSlots[i].itemId;

            GameObject handObject = GetHandItemObject(itemId);
            GameObject worldPrefab = GetWorldPickupPrefab(itemId);

            if (handObject == null)
            {
                Debug.LogWarning($"슬롯 복구 실패: {itemId}");
                continue;
            }

            slots[i].itemId = itemId;
            slots[i].itemName = savedSlots[i].itemName;
            slots[i].itemObject = handObject;
            slots[i].worldPickupPrefab = worldPrefab;
            slots[i].unlocked = true;
            slots[i].stackable = savedSlots[i].stackable;
            slots[i].count = savedSlots[i].count;
            slots[i].maxStack = savedSlots[i].maxStack;

            slots[i].itemObject.SetActive(false);
        }

        currentIndex = Mathf.Clamp(savedCurrentIndex, 0, slots.Length - 1);

        if (!slots[currentIndex].unlocked)
            SelectNextAvailableSlot();
        else
            UpdateItemVisibility();

        Debug.Log("저장된 인벤토리 복구 완료");
    }
}