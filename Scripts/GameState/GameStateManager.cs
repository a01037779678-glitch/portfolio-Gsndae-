using UnityEngine;
using System.Collections.Generic;
using System;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("씬 이동 위치 저장")]
    public string nextSpawnPointName = "";

    [Header("획득한 월드 아이템 저장")]
    public List<string> pickedWorldItemIds = new List<string>();

    [Header("House 1 상태")]
    public bool pickedHouse1Item = false;
    public bool house1ReEnterEventDone = false;

    [Header("House 2 상태")]
    public bool house2LivingRoomDollPicked = false;
    public bool house2EnemyFirstEventDone = false;
    public bool house2EnemyOutsideChasing = false;

    [Header("House 2 - 정화 상태")]
    public bool house2EnemyPurifiedByTotem = false;

    [Header("House 2 - 2번째 공간 상태")]
    public bool house2SecondAreaEventStarted = false;
    public bool house2TalismanRemoved = false;
    public bool house2ThirdAreaKeyPicked = false;

    [Header("House 2 - 3번째 공간 상태")]
    public bool house2FinalTotemPicked = false;

    [Header("House 3 상태")]
    public bool house3FirstPuzzleSolved = false;    // 첫 번째 토템 퍼즐 해결
    public bool house3FirstTotemPicked = false;      // 첫 번째 토템 획득
    public bool house3FirstTotemPlaced = false;      // 밖 제단에 첫 번째 토템 배치
    public bool house3SecondPhaseStarted = false;    // 그슨대 추격 구간 시작

    public bool house3Seal1Broken = false;           // 봉인 장치 1 해제
    public bool house3Seal2Broken = false;           // 봉인 장치 2 해제
    public bool house3Seal3Broken = false;           // 봉인 장치 3 해제

    public bool house3FinalTotemUnlocked = false;    // 두 번째 토템방 개방
    public bool house3FinalTotemPicked = false;      // 두 번째 토템 획득


    [Header("인형 제단 상태")]
    public bool[] dollAltars = new bool[5];
    public int placedDollCount = 0;

    [Header("진엔딩 - 가재 제단 상태")]
    public bool[] crawfishAltars = new bool[3];
    public int placedCrawfishCount = 0;
    public bool trueEndingUnlocked = false;

    public static event Action OnTotemProgressChanged;
    [Header("플레이어 상태")]
    public bool flashlightLocked = false;
    public bool torchLocked = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // =========================
    // 씬 이동
    // =========================

    public void SetNextSpawnPoint(string spawnPointName)
    {
        nextSpawnPointName = spawnPointName;
        Debug.Log("다음 스폰 위치 저장: " + spawnPointName);
    }

    // =========================
    // 월드 아이템 저장
    // =========================

    public bool IsWorldItemPicked(string worldItemId)
    {
        if (string.IsNullOrEmpty(worldItemId))
            return false;

        return pickedWorldItemIds.Contains(worldItemId);
    }

    public void SetWorldItemPicked(string worldItemId)
    {
        if (string.IsNullOrEmpty(worldItemId))
            return;

        if (!pickedWorldItemIds.Contains(worldItemId))
        {
            pickedWorldItemIds.Add(worldItemId);
            Debug.Log("월드 아이템 획득 저장: " + worldItemId);
        }
    }

    // =========================
    // House 1
    // =========================

    public void SetPickedHouse1Item()
    {
        pickedHouse1Item = true;
        Debug.Log("저장됨: 집1 아이템 획득");
    }

    public void SetHouse1ReEnterEventDone()
    {
        house1ReEnterEventDone = true;
        Debug.Log("저장됨: 집1 재입장 이벤트 완료");
    }

    // =========================
    // House 2
    // =========================

    public void SetHouse2LivingRoomDollPicked()
    {
        house2LivingRoomDollPicked = true;
        Debug.Log("저장됨: 집2 메인 토템 획득");
    }

    public void SetHouse2EnemyFirstEventDone()
    {
        house2EnemyFirstEventDone = true;
        Debug.Log("저장됨: 집2 그슨대 첫 등장 이벤트 완료");
    }

    public void StartHouse2EnemyOutsideChase()
    {
        if (house2EnemyPurifiedByTotem)
        {
            house2EnemyOutsideChasing = false;
            Debug.Log("집2 그슨대는 이미 토템으로 정화됨 → 밖 추격 시작 안 함");
            return;
        }

        house2EnemyOutsideChasing = true;
        Debug.Log("저장됨: 집2 그슨대 밖 추격 시작");
    }

    public void StopHouse2EnemyOutsideChase()
    {
        house2EnemyOutsideChasing = false;
        Debug.Log("저장됨: 집2 그슨대 밖 추격 종료");
    }

    public void SetHouse2EnemyPurifiedByTotem()
    {
        house2EnemyPurifiedByTotem = true;
        house2EnemyOutsideChasing = false;
        Debug.Log("저장됨: 집2 그슨대 토템으로 정화 완료");
    }

    public bool IsHouse2EnemyPurifiedByTotem()
    {
        return house2EnemyPurifiedByTotem;
    }

    public void SetHouse2SecondAreaEventStarted()
    {
        house2SecondAreaEventStarted = true;
        Debug.Log("저장됨: 집2 2번째 공간 이벤트 시작");
    }

    public void SetHouse2TalismanRemoved()
    {
        house2TalismanRemoved = true;
        Debug.Log("저장됨: 집2 부적 제거 완료");
    }

    public void SetHouse2ThirdAreaKeyPicked()
    {
        house2ThirdAreaKeyPicked = true;
        Debug.Log("저장됨: 집2 3번째 공간 열쇠 획득");
    }

    public void SetHouse2FinalTotemPicked()
    {
        house2FinalTotemPicked = true;
        Debug.Log("저장됨: 집2 3번째 공간 최종 토템 획득");
    }

    // =========================
    // House 3
    // =========================

    public void SetHouse3FirstPuzzleSolved()
    {
        house3FirstPuzzleSolved = true;
        Debug.Log("저장됨: 집3 첫 번째 퍼즐 해결");
    }
    public void SetHouse3FirstTotemPicked()
    {
        house3FirstTotemPicked = true;
        Debug.Log("저장됨: 집3 첫 번째 토템 획득");
    }

    public void SetHouse3FirstTotemPlaced()
    {
        house3FirstTotemPlaced = true;
        Debug.Log("저장됨: 집3 첫 번째 토템 제단 배치");
    }

    public void StartHouse3SecondPhase()
    {
        house3SecondPhaseStarted = true;
        Debug.Log("저장됨: 집3 2차 구간 시작");
    }

    public void BreakHouse3Seal(int sealIndex)
    {
        switch (sealIndex)
        {
            case 1:
                house3Seal1Broken = true;
                break;

            case 2:
                house3Seal2Broken = true;
                break;

            case 3:
                house3Seal3Broken = true;
                break;

            default:
                Debug.LogWarning($"집3 봉인 번호가 잘못됨: {sealIndex}");
                return;
        }

        Debug.Log($"저장됨: 집3 봉인 {sealIndex} 해제");

        if (IsAllHouse3SealsBroken())
        {
            UnlockHouse3FinalTotem();
        }
    }

    public bool IsHouse3SealBroken(int sealIndex)
    {
        switch (sealIndex)
        {
            case 1:
                return house3Seal1Broken;

            case 2:
                return house3Seal2Broken;

            case 3:
                return house3Seal3Broken;

            default:
                return false;
        }
    }

    public bool IsAllHouse3SealsBroken()
    {
        return house3Seal1Broken &&
               house3Seal2Broken &&
               house3Seal3Broken;
    }

    public int GetHouse3BrokenSealCount()
    {
        int count = 0;

        if (house3Seal1Broken) count++;
        if (house3Seal2Broken) count++;
        if (house3Seal3Broken) count++;

        return count;
    }

    public void UnlockHouse3FinalTotem()
    {
        if (house3FinalTotemUnlocked)
            return;

        house3FinalTotemUnlocked = true;
    }

    public void SetHouse3FinalTotemPicked()
    {
        house3FinalTotemPicked = true;
    }

    [ContextMenu("Reset House3 Test State")]
    public void ResetHouse3TestState()
    {
        house3FirstPuzzleSolved = false;
        house3FirstTotemPicked = false;
        house3FirstTotemPlaced = false;
        house3SecondPhaseStarted = false;

        house3Seal1Broken = false;
        house3Seal2Broken = false;
        house3Seal3Broken = false;

        house3FinalTotemUnlocked = false;
        house3FinalTotemPicked = false;

    }
    // =========================
    // 테스트용 초기화
    // =========================

    [ContextMenu("Reset House2 Test State")]
    public void ResetHouse2TestState()
    {
        house2LivingRoomDollPicked = false;
        house2EnemyFirstEventDone = false;
        house2EnemyOutsideChasing = false;
        house2EnemyPurifiedByTotem = false;

        house2SecondAreaEventStarted = false;
        house2TalismanRemoved = false;
        house2ThirdAreaKeyPicked = false;
        house2FinalTotemPicked = false;

    }

    // =========================
    // 인형 제단
    // =========================

    public bool IsAltarPlaced(int altarIndex)
    {
        if (altarIndex < 0 || altarIndex >= dollAltars.Length)
            return false;

        return dollAltars[altarIndex];
    }

    public void PlaceDollOnAltar(int altarIndex)
    {
        if (altarIndex < 0 || altarIndex >= dollAltars.Length)
            return;

        if (dollAltars[altarIndex])
            return;

        dollAltars[altarIndex] = true;
        placedDollCount++;

        Debug.Log($"제단 {altarIndex}에 인형 배치됨. 현재 개수: {placedDollCount}/{dollAltars.Length}");

        // 이벤트 방식
        OnTotemProgressChanged?.Invoke();

        // 직접 갱신 방식
        RefreshHouseEntranceLocks();
    }
    public void RefreshHouseEntranceLocks()
    {
        HouseEntranceLock[] locks =
            FindObjectsByType<HouseEntranceLock>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        for (int i = 0; i < locks.Length; i++)
        {
            if (locks[i] == null)
                continue;

            locks[i].RefreshLockState();
        }
    }
    public void ResetAltars()
    {
        for (int i = 0; i < dollAltars.Length; i++)
        {
            dollAltars[i] = false;
        }

        placedDollCount = 0;

        OnTotemProgressChanged?.Invoke();
        RefreshHouseEntranceLocks();
    }


    // =========================
    // 플레이어 상태
    // =========================

    public void LockFlashlight()
    {
        flashlightLocked = true;
    }

    public void UnlockFlashlight()
    {
        flashlightLocked = false;
    }

    public void LockTorch()
    {
        torchLocked = true;
    }

    public void UnlockTorch()
    {
        torchLocked = false;
    }

    // =========================
    // 진엔딩 - 가재 제단
    // =========================

    public bool IsCrawfishAltarPlaced(int altarIndex)
    {
        if (altarIndex < 0 || altarIndex >= crawfishAltars.Length)
            return false;

        return crawfishAltars[altarIndex];
    }

    public void PlaceCrawfishOnAltar(int altarIndex)
    {
        if (altarIndex < 0 || altarIndex >= crawfishAltars.Length)
            return;

        if (crawfishAltars[altarIndex])
            return;

        crawfishAltars[altarIndex] = true;
        placedCrawfishCount++;

        if (placedCrawfishCount >= crawfishAltars.Length)
        {
            trueEndingUnlocked = true;
            Debug.Log("진엔딩 조건 달성!");
        }
    }

    public bool IsTrueEndingUnlocked()
    {
        return trueEndingUnlocked;
    }

    public void ResetCrawfishAltars()
    {
        for (int i = 0; i < crawfishAltars.Length; i++)
        {
            crawfishAltars[i] = false;
        }

        placedCrawfishCount = 0;
        trueEndingUnlocked = false;

        Debug.Log("가재 제단 상태 초기화");
    }

    public void ApplySaveData(SaveData data)
    {
        if (data == null)
            return;

        nextSpawnPointName = data.nextSpawnPointName;

        pickedWorldItemIds = new List<string>(data.pickedWorldItemIds);

        pickedHouse1Item = data.pickedHouse1Item;
        house1ReEnterEventDone = data.house1ReEnterEventDone;

        house2LivingRoomDollPicked = data.house2LivingRoomDollPicked;
        house2EnemyFirstEventDone = data.house2EnemyFirstEventDone;
        house2EnemyOutsideChasing = data.house2EnemyOutsideChasing;
        house2EnemyPurifiedByTotem = data.house2EnemyPurifiedByTotem;

        house2SecondAreaEventStarted = data.house2SecondAreaEventStarted;
        house2TalismanRemoved = data.house2TalismanRemoved;
        house2ThirdAreaKeyPicked = data.house2ThirdAreaKeyPicked;
        house2FinalTotemPicked = data.house2FinalTotemPicked;

        if (data.dollAltars != null && data.dollAltars.Length == dollAltars.Length)
        {
            for (int i = 0; i < dollAltars.Length; i++)
            {
                dollAltars[i] = data.dollAltars[i];
            }
        }

        placedDollCount = data.placedDollCount;

        if (data.crawfishAltars != null && data.crawfishAltars.Length == crawfishAltars.Length)
        {
            for (int i = 0; i < crawfishAltars.Length; i++)
            {
                crawfishAltars[i] = data.crawfishAltars[i];
            }
        }

        placedCrawfishCount = data.placedCrawfishCount;
        trueEndingUnlocked = data.trueEndingUnlocked;

        flashlightLocked = data.flashlightLocked;
        torchLocked = data.torchLocked;

        Debug.Log("세이브 데이터 적용 완료");
    }
}