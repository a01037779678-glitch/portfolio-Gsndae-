using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerSceneSpawner : MonoBehaviour
{
    private CharacterController controller;
    private ItemSwitcher itemSwitcher;

    [Header("스폰 위치 보정")]
    public float spawnHeightOffset = 0.2f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        itemSwitcher = GetComponent<ItemSwitcher>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        StartCoroutine(MovePlayerToSpawnPointNextFrame());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(MovePlayerToSpawnPointNextFrame());
    }

    private IEnumerator MovePlayerToSpawnPointNextFrame()
    {
        yield return null;
        yield return null;

        MovePlayerToSpawnPoint();
    }

    private void MovePlayerToSpawnPoint()
    {
        if (GameStateManager.Instance == null)
        {
            RefreshHeldItem();
            return;
        }

        string targetName = GameStateManager.Instance.nextSpawnPointName;

        if (string.IsNullOrEmpty(targetName))
        {
            RefreshHeldItem();
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();

        SpawnPoint[] spawnPoints =
            UnityEngine.Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

        SpawnPoint targetSpawnPoint = null;

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint == null)
                continue;

            if (spawnPoint.gameObject.scene != activeScene)
                continue;

            if (spawnPoint.spawnPointName == targetName)
            {
                targetSpawnPoint = spawnPoint;
                break;
            }
        }

        if (targetSpawnPoint == null)
        {
            Debug.LogWarning(
                $"스폰포인트를 찾지 못함. Target: {targetName}, Active Scene: {activeScene.name}"
            );

            foreach (SpawnPoint spawnPoint in spawnPoints)
            {
                if (spawnPoint != null && spawnPoint.gameObject.scene == activeScene)
                {
                    Debug.Log(
                        $"현재 씬 SpawnPoint: Object={spawnPoint.name}, Name={spawnPoint.spawnPointName}, Pos={spawnPoint.transform.position}"
                    );
                }
            }

            RefreshHeldItem();
            return;
        }

        MoveToSpawn(targetSpawnPoint);

        Debug.Log(
            $"플레이어 스폰 위치 이동: {targetName}, Scene: {activeScene.name}, Position: {targetSpawnPoint.transform.position}"
        );

        GameStateManager.Instance.nextSpawnPointName = "";

        RefreshHeldItem();
    }

    private void MoveToSpawn(SpawnPoint spawnPoint)
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        Vector3 targetPosition = spawnPoint.transform.position + Vector3.up * spawnHeightOffset;
        Quaternion targetRotation = spawnPoint.transform.rotation;

        if (controller != null)
            controller.enabled = false;

        transform.SetPositionAndRotation(targetPosition, targetRotation);

        Physics.SyncTransforms();

        if (controller != null)
        {
            controller.enabled = true;
            controller.Move(Vector3.zero);
        }
    }

    private void RefreshHeldItem()
    {
        if (itemSwitcher == null)
            itemSwitcher = GetComponent<ItemSwitcher>();

        if (itemSwitcher != null)
            // 아이템의 표시 상태만 복원
            itemSwitcher.UpdateItemVisibility();
    }
}