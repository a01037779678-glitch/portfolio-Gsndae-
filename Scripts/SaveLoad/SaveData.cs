using System.Collections.Generic;



[System.Serializable]
public class SavedItemSlot
{
    public string itemId;
    public string itemName;
    public bool unlocked;

    public bool stackable;
    public int count;
    public int maxStack;
}
public class SaveData
{
    // 현재 씬
    public string currentSceneName;

    // 씬 이동용 스폰포인트
    public string nextSpawnPointName;

    // 월드 아이템
    public List<string> pickedWorldItemIds = new List<string>();

    // House 1
    public bool pickedHouse1Item;
    public bool house1ReEnterEventDone;

    // House 2
    public bool house2LivingRoomDollPicked;
    public bool house2EnemyFirstEventDone;
    public bool house2EnemyOutsideChasing;
    public bool house2EnemyPurifiedByTotem;

    public bool house2SecondAreaEventStarted;
    public bool house2TalismanRemoved;
    public bool house2ThirdAreaKeyPicked;
    public bool house2FinalTotemPicked;

    // House 3
    public bool house3FirstPuzzleSolved;
    public bool house3FirstTotemPicked;
    public bool house3FirstTotemPlaced;
    public bool house3SecondPhaseStarted;

    public bool house3Seal1Broken;
    public bool house3Seal2Broken;
    public bool house3Seal3Broken;

    public bool house3FinalTotemUnlocked;
    public bool house3FinalTotemPicked;

    // 제단
    public bool[] dollAltars;
    public int placedDollCount;
    public bool[] crawfishAltars;
    public int placedCrawfishCount;
    public bool trueEndingUnlocked;

    // 플레이어 상태
    public bool flashlightLocked;
    public bool torchLocked;

    // 현재 손에 든 아이템
    public SavedItemSlot[] itemSlots;
    public int currentItemIndex;
}