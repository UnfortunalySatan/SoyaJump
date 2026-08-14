using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [Header("Платформы")]
    [SerializeField] private GameObject[] platformsPrefabs;
    [SerializeField] private int poolSizeType = 10;
    [SerializeField] private float[] spawnChances;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float minDistanceY = 0.3f;
    [SerializeField] private float maxDistanceY = 0.7f;
    [SerializeField] private int initialSpawnCount = 15; // Сколько платформ спавнить на старте
    [Header("Дальность генерации")]
    [SerializeField] private float spawnAheadDistance = 5f;
    [Header("Сложность")]
    [SerializeField] private int hardDifficult = 1000;
    [SerializeField] private int midDifficult = 500;

    private List<List<GameObject>> multiPool = new List<List<GameObject>>();
    private Vector3 oldPosition;

    private float leftBorder, rightBorder;
    private Camera cam;

    // Ссылка на корутину, чтобы корректно управлять перезапуском игры
    private Coroutine initAndSpawnRoutine;
    float platformWidth;
    private void Awake()
    {
        // Просто объявляем камеру и границы заранее
        cam = Camera.main;
        GetScreenBorders();
    }

    private void Start()
    {
        // Запускаем единую цепочку: Инициализация пула -> Спавн первых платформ
        initAndSpawnRoutine = StartCoroutine(InitializeAndStartGame());
        PlatformWidth();
    }

    private IEnumerator InitializeAndStartGame()
    {
        oldPosition = startPosition;

        // 1. Создаем пул объектов
        for (int i = 0; i < platformsPrefabs.Length; i++)
        {
            List<GameObject> subPool = new List<GameObject>();
            for (int j = 0; j < poolSizeType; j++)
            {
                GameObject obj = Instantiate(platformsPrefabs[i]);
                obj.SetActive(false);
                subPool.Add(obj);
            }
            multiPool.Add(subPool);
        }

        // Ждем один кадр, чтобы всё точно создалось
        yield return null;

        // 2. Генерируем стартовые 15 платформ друг за другом
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnPlatform(PlatformPosition());
        }
    }

    public void SpawnPlatform(Vector3 spawnPos)
    {
        int targetType = GetRandomTypeIndex();
        GameObject platform = GetObjectFromSubPool(targetType);
        if (platform != null)
        {
            platform.transform.position = spawnPos;
            platform.SetActive(true);
            platform.GetComponent<Platform>().TouchedReset();
            
            oldPosition = spawnPos;
        }
    }

    private int GetRandomTypeIndex()
    {
        float totalWeight = 0f;
        foreach (float chance in spawnChances) totalWeight += chance;

        float randomValue = Random.Range(0, totalWeight);
        float currentWeightSum = 0f;
        for (int i = 0; i < spawnChances.Length; i++)
        {
            currentWeightSum += spawnChances[i];
            if (randomValue <= currentWeightSum) return i;
        }
        return spawnChances.Length - 1;
    }

    private GameObject GetObjectFromSubPool(int typeIndex)
    {
        List<GameObject> subPool = multiPool[typeIndex];
        foreach (GameObject obj in subPool)
        {
            if (!obj.activeInHierarchy) return obj;
        }
        GameObject newObj = Instantiate(platformsPrefabs[typeIndex]);
        newObj.SetActive(false);
        subPool.Add(newObj);
        return newObj;
    }

    private float PosY()
    {
        float yRand = Random.Range(minDistanceY, maxDistanceY);
        return yRand + oldPosition.y;
    }

    private float PosX()
    {
        float xPos = Random.Range(leftBorder, rightBorder);
        //float platformWidth = platformsPrefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;
        xPos = Mathf.Clamp(xPos, leftBorder + platformWidth / 2, rightBorder - platformWidth / 2);
        return xPos;
    }

    private Vector3 PlatformPosition()
    {
        return new Vector3(PosX(), PosY(), 0f);
    }

    public void GetScreenBorders()
    {
        Vector3 leftEdge = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 rightEdge = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        leftBorder = leftEdge.x;
        rightBorder = rightEdge.x;
        EventBus.isGetScreenBorders?.Invoke(rightBorder, leftBorder);
    }

    // ИСПРАВЛЕНО ДЛЯ ПЕРЕЗАПУСКА
    public void ResetSpawner()
    {
        if (initAndSpawnRoutine != null) StopCoroutine(initAndSpawnRoutine);

        oldPosition = startPosition;
        foreach (List<GameObject> subPool in multiPool)
        {
            foreach (GameObject platform in subPool)
            {
                platform.SetActive(false);
            }
        }

        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnPlatform(PlatformPosition());
        }
    }

    public void UpdateChances(int currentScore)
    {
        if (currentScore >= hardDifficult)
        {
            spawnChances[0] = 20f; spawnChances[1] = 40f; spawnChances[2] = 40f;
        }
        else if (currentScore >= midDifficult)
        {
            spawnChances[0] = 50f; spawnChances[1] = 30f; spawnChances[2] = 20f;
        }
        else
        {
            spawnChances[0] = 65f; spawnChances[1] = 20f; spawnChances[2] = 15f;
        }
    }

    private void Update()
    {
        if (multiPool.Count == 0) return;

        float screenTopY = cam.transform.position.y + cam.orthographicSize;
        while(oldPosition.y < screenTopY + spawnAheadDistance)
        {
            SpawnPlatform(PlatformPosition());
        }
    }

    private void PlatformWidth()
    {
        platformWidth = platformsPrefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;
        EventBus.isPlatformWidth?.Invoke(platformWidth);
    }

    private void OnEnable()
    {
        EventBus.isPlayerReady += ResetSpawner;
    }
    private void OnDisable()
    {
        EventBus.isPlayerReady -= ResetSpawner;
    }
}
