using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuManager : MonoBehaviour
{
    [Header("죽었을 때 보여줄 UI")]
    public GameObject deathPanel;

    [Header("옵션")]
    public bool autoFindDeathPanel = true;

    private bool isShowing = false;

    public string mainMenuSceneName = "TitleScreen";

    private Transform originalCameraParent;
    private Vector3 originalCameraLocalPosition;
    private Quaternion originalCameraLocalRotation;

    private void Awake()
    {
        FindDeathPanelIfNeeded();

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    private void Start()
    {
        FindDeathPanelIfNeeded();

        if (deathPanel != null)
            deathPanel.SetActive(false);

        SaveOriginalCameraTransform();
    }
    private void SaveOriginalCameraTransform()
    {
        PlayerController player =
            Object.FindFirstObjectByType<PlayerController>();

        if (player == null || player.cameraTransform == null)
            return;

        Transform cam = player.cameraTransform;

        originalCameraParent = cam.parent;
        originalCameraLocalPosition = cam.localPosition;
        originalCameraLocalRotation = cam.localRotation;

        Debug.Log("플레이어 카메라 원래 Transform 저장 완료");
    }
    private void FindDeathPanelIfNeeded()
    {
        if (!autoFindDeathPanel)
            return;

        if (deathPanel != null)
            return;

        DeathPanelMarker marker =
            Object.FindFirstObjectByType<DeathPanelMarker>(FindObjectsInactive.Include);

        if (marker != null)
        {
            deathPanel = marker.gameObject;
            Debug.Log("DeathMenuManager: DeathPanel 자동 연결 완료");
        }
        else
        {
            Debug.LogWarning("DeathMenuManager: DeathPanelMarker가 붙은 DeathPanel을 찾지 못했습니다.");
        }
    }

    public void ShowDeathMenu()
    {
        Debug.Log("DeathMenuManager: ShowDeathMenu 호출됨");

        if (isShowing)
        {
            Debug.Log("DeathMenuManager: 이미 표시 중이라 return됨");
            return;
        }

        isShowing = true;

        DeathPanelMarker[] markers =
            Object.FindObjectsByType<DeathPanelMarker>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        Debug.Log($"DeathMenuManager: 찾은 DeathPanelMarker 개수 = {markers.Length}");

        if (markers.Length == 0)
        {
            Debug.LogWarning("DeathMenuManager: DeathPanelMarker가 붙은 오브젝트가 없습니다.");
        }

        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i] == null)
                continue;

            GameObject panel = markers[i].gameObject;

            Debug.Log(
                $"DeathPanel 후보 {i}: " +
                $"name={panel.name}, " +
                $"activeSelf={panel.activeSelf}, " +
                $"activeInHierarchy={panel.activeInHierarchy}, " +
                $"parent={(panel.transform.parent != null ? panel.transform.parent.name : "None")}"
            );

            // 부모들도 켜기
            Transform current = panel.transform.parent;
            while (current != null)
            {
                if (!current.gameObject.activeSelf)
                {
                    Debug.Log($"부모 오브젝트 활성화: {current.name}");
                    current.gameObject.SetActive(true);
                }

                current = current.parent;
            }

            panel.SetActive(true);

            Debug.Log(
                $"DeathPanel 활성화 후 {i}: " +
                $"name={panel.name}, " +
                $"activeSelf={panel.activeSelf}, " +
                $"activeInHierarchy={panel.activeInHierarchy}"
            );

            deathPanel = panel;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 1f;
    }
    public void RetryFromLastSave()
    {
        Debug.Log("========== 다시하기 시작 ==========");

        Time.timeScale = 1f;

        HideAllDeathPanels();

        SaveData data = SaveManager.Load();

        if (data == null)
        {
            Debug.LogWarning("저장 데이터가 없습니다.");

            RevivePlayerBeforeReload();

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name
            );

            return;
        }

        // -------------------------
        // 1. GameState 복구
        // -------------------------

        ApplySaveDataToGameState(data);

        // -------------------------
        // 2. 인벤토리 복구
        // -------------------------

        ItemSwitcher itemSwitcher =
            Object.FindFirstObjectByType<ItemSwitcher>();

        if (itemSwitcher != null)
        {
            itemSwitcher.RestoreSlots(
                data.itemSlots,
                data.currentItemIndex
            );

            Debug.Log("저장 당시 인벤토리 복구 완료");

            for (int i = 0; i < itemSwitcher.slots.Length; i++)
            {
                Debug.Log(
                    $"복구 슬롯 {i}: " +
                    $"unlocked={itemSwitcher.slots[i].unlocked}, " +
                    $"itemId={itemSwitcher.slots[i].itemId}"
                );
            }
        }
        else
        {
            Debug.LogWarning("ItemSwitcher를 찾지 못했습니다.");
        }

        // -------------------------
        // 3. 죽음 상태 / 카메라 복구
        // -------------------------

        RevivePlayerBeforeReload();

        // -------------------------
        // 4. 저장 SpawnPoint 복구
        // -------------------------

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.nextSpawnPointName =
                data.nextSpawnPointName;
        }

        // -------------------------
        // 5. 저장 씬 로드
        // -------------------------

        string sceneToLoad = data.currentSceneName;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            sceneToLoad =
                SceneManager.GetActiveScene().name;
        }

        Debug.Log(
            $"마지막 저장으로 복귀: " +
            $"Scene={sceneToLoad}, " +
            $"Spawn={data.nextSpawnPointName}"
        );

        SceneManager.LoadScene(sceneToLoad);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        HideAllDeathPanels();   

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 플레이어가 DontDestroyOnLoad라면 메인 화면까지 따라오지 않게 제거
        if (PlayerPersistent.Instance != null)
        {
            Destroy(PlayerPersistent.Instance.gameObject);
        }

        // GameStateManager도 DontDestroyOnLoad라서 메인 화면에서 필요 없으면 제거
        if (GameStateManager.Instance != null)
        {
            Destroy(GameStateManager.Instance.gameObject);
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
    private void ApplySaveDataToGameState(SaveData data)
    {
        if (GameStateManager.Instance == null || data == null)
            return;

        GameStateManager gsm = GameStateManager.Instance;

        gsm.nextSpawnPointName = data.nextSpawnPointName;

        gsm.pickedWorldItemIds = new System.Collections.Generic.List<string>(data.pickedWorldItemIds);

        gsm.pickedHouse1Item = data.pickedHouse1Item;
        gsm.house1ReEnterEventDone = data.house1ReEnterEventDone;

        gsm.house2LivingRoomDollPicked = data.house2LivingRoomDollPicked;
        gsm.house2EnemyFirstEventDone = data.house2EnemyFirstEventDone;
        gsm.house2EnemyOutsideChasing = data.house2EnemyOutsideChasing;
        gsm.house2EnemyPurifiedByTotem = data.house2EnemyPurifiedByTotem;

        gsm.house2SecondAreaEventStarted = data.house2SecondAreaEventStarted;
        gsm.house2TalismanRemoved = data.house2TalismanRemoved;
        gsm.house2ThirdAreaKeyPicked = data.house2ThirdAreaKeyPicked;
        gsm.house2FinalTotemPicked = data.house2FinalTotemPicked;

        if (data.dollAltars != null)
            gsm.dollAltars = (bool[])data.dollAltars.Clone();

        gsm.placedDollCount = data.placedDollCount;

        gsm.flashlightLocked = data.flashlightLocked;
        gsm.torchLocked = data.torchLocked;

        Debug.Log("마지막 저장 데이터로 GameStateManager 복구 완료");
    }

    private void RevivePlayerBeforeReload()
    {
        PlayerController player =
            Object.FindFirstObjectByType<PlayerController>();

        if (player == null)
            return;

        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = true;

        player.enabled = true;

        // 죽음 연출로 움직인 실제 플레이어 카메라 복원
        if (player.cameraTransform != null)
        {
            Transform camTransform = player.cameraTransform;

            if (originalCameraParent != null)
                camTransform.SetParent(originalCameraParent, false);

            camTransform.localPosition = originalCameraLocalPosition;
            camTransform.localRotation = originalCameraLocalRotation;

            Camera cam = camTransform.GetComponent<Camera>();

            if (cam != null)
                cam.fieldOfView = 60f;

            Debug.Log("플레이어 카메라 위치/회전 복구 완료");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void HideAllDeathPanels()
    {
        DeathPanelMarker[] markers =
            Object.FindObjectsByType<DeathPanelMarker>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i] == null)
                continue;

            markers[i].gameObject.SetActive(false);
        }

        if (deathPanel != null)
            deathPanel.SetActive(false);

        isShowing = false;

        Debug.Log("DeathMenuManager: DeathPanel 전체 비활성화 완료");
    }       
}