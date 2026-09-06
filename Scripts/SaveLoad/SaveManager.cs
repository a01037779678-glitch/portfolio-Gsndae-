using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public static class SaveManager
{
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "saveData.json");
    public static bool HasSaveFile()
    {
        return File.Exists(SavePath);
    }

    public static void Save(string sceneNameToSave = "")
    {
        if (GameStateManager.Instance == null)
        {
            return;
        }

        GameStateManager gsm = GameStateManager.Instance;

        SaveData data = new SaveData();

        if (string.IsNullOrEmpty(sceneNameToSave))
            data.currentSceneName = SceneManager.GetActiveScene().name;
        else
            data.currentSceneName = sceneNameToSave;

        //다음 스폰포인트 이름 저장
        data.nextSpawnPointName = gsm.nextSpawnPointName;

        //World Item ID 리스트는 새로운 List로 복사해서 저장 (참조 문제 방지)
        data.pickedWorldItemIds = new System.Collections.Generic.List<string>(gsm.pickedWorldItemIds);

        //house1 관련 이벤트 상태 저장
        data.pickedHouse1Item = gsm.pickedHouse1Item;
        data.house1ReEnterEventDone = gsm.house1ReEnterEventDone;

        //house2 관련 이벤트 상태 저장
        data.house2LivingRoomDollPicked = gsm.house2LivingRoomDollPicked;
        data.house2EnemyFirstEventDone = gsm.house2EnemyFirstEventDone;
        data.house2EnemyOutsideChasing = gsm.house2EnemyOutsideChasing;
        data.house2EnemyPurifiedByTotem = gsm.house2EnemyPurifiedByTotem;

        //house2 관련 이벤트 상태 저장
        data.house2SecondAreaEventStarted = gsm.house2SecondAreaEventStarted;
        data.house2TalismanRemoved = gsm.house2TalismanRemoved;
        data.house2ThirdAreaKeyPicked = gsm.house2ThirdAreaKeyPicked;
        data.house2FinalTotemPicked = gsm.house2FinalTotemPicked;

        //house3 관련 이벤트 상태 저장
        data.house3FirstPuzzleSolved = gsm.house3FirstPuzzleSolved;
        data.house3FirstTotemPicked = gsm.house3FirstTotemPicked;
        data.house3FirstTotemPlaced = gsm.house3FirstTotemPlaced;
        data.house3SecondPhaseStarted = gsm.house3SecondPhaseStarted;
        //house3 봉인 상태 저장
        data.house3Seal1Broken = gsm.house3Seal1Broken;
        data.house3Seal2Broken = gsm.house3Seal2Broken;
        data.house3Seal3Broken = gsm.house3Seal3Broken;
        //house3 최종 토템 상태 저장    
        data.house3FinalTotemUnlocked = gsm.house3FinalTotemUnlocked;
        data.house3FinalTotemPicked = gsm.house3FinalTotemPicked;
        //인형 제단
        data.dollAltars = (bool[])gsm.dollAltars.Clone();
        data.placedDollCount = gsm.placedDollCount;

        //가재 제단
        data.crawfishAltars = (bool[])gsm.crawfishAltars.Clone();
        data.placedCrawfishCount = gsm.placedCrawfishCount;
        data.trueEndingUnlocked = gsm.trueEndingUnlocked;

        //플래시라이트/횃불 잠금 상태 저장
        data.flashlightLocked = gsm.flashlightLocked;
        data.torchLocked = gsm.torchLocked;

        // 인벤토리 저장은 ToJson 전에 해야 함
        ItemSwitcher itemSwitcher = UnityEngine.Object.FindFirstObjectByType<ItemSwitcher>();


        if (itemSwitcher != null)
        {
            SavedItemSlot[] originalSlots = itemSwitcher.GetSavedSlots();
            SavedItemSlot[] filteredSlots = new SavedItemSlot[originalSlots.Length];

            for (int i = 0; i < originalSlots.Length; i++)
            {
                if (originalSlots[i] == null)
                    continue;

                if (!originalSlots[i].unlocked)
                    continue;

                if (!ShouldSaveItem(originalSlots[i].itemId))
                    continue;

                filteredSlots[i] = originalSlots[i];
            }

            data.itemSlots = filteredSlots;

            // 현재 들고 있던 아이템이 저장 대상이면 그 슬롯 유지,
            // 토템 같은 저장 제외 아이템이면 손전등/횃불 중 첫 번째 슬롯으로 바꿈
            data.currentItemIndex = 0;

            if (itemSwitcher.currentIndex >= 0 &&
                itemSwitcher.currentIndex < originalSlots.Length &&
                originalSlots[itemSwitcher.currentIndex] != null &&
                ShouldSaveItem(originalSlots[itemSwitcher.currentIndex].itemId))
            {
                data.currentItemIndex = itemSwitcher.currentIndex;
            }
            else
            {
                for (int i = 0; i < filteredSlots.Length; i++)
                {
                    if (filteredSlots[i] != null &&
                        filteredSlots[i].unlocked &&
                        ShouldSaveItem(filteredSlots[i].itemId))
                    {
                        data.currentItemIndex = i;
                        break;
                    }
                }
            }

        }
        else
        {
            data.itemSlots = new SavedItemSlot[0];
            data.currentItemIndex = 0;

        }

        // 모든 데이터를 넣은 뒤 JSON 생성/저장
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }
    public static SaveData Load()
    {
        if (!HasSaveFile())
        {
            return null;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return data;
    }
    private static bool ShouldSaveItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return false;

        return itemId == "Flashlight" ||
               itemId == "Torch";
    }

    public static void DeleteSave()
    {
        if (!HasSaveFile())
            return;

        File.Delete(SavePath);
    }
}