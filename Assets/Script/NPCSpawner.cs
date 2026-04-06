using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject[] npCars;          // mảng prefab (chỉ để tham chiếu loại xe)
    public Transform laneLeft;
    public Transform laneRight;
    public float minSpawnTime = 2f;
    public float maxSpawnTime = 5f;
    public int maxCars = 5;
    public int currentCars;

    // Tham số thay đổi theo độ khó
    private float currentMinSpawn;
    private float currentMaxSpawn;
    private int currentMaxCars;

    void Start()
    {
        currentMinSpawn = minSpawnTime;
        currentMaxSpawn = maxSpawnTime;
        currentMaxCars = maxCars;
        StartCoroutine(SpawnCarRoutine());
        StartCoroutine(IncreaseDifficultyOverTime());
    }

    IEnumerator SpawnCarRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(currentMinSpawn, currentMaxSpawn);
            yield return new WaitForSeconds(waitTime);
            if (currentCars < currentMaxCars)
            {
                SpawnCar();
            }
        }
    }

    IEnumerator IncreaseDifficultyOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(30f); // mỗi 30 giây
            currentMinSpawn = Mathf.Max(0.5f, currentMinSpawn - 0.2f);
            currentMaxSpawn = Mathf.Max(1.2f, currentMaxSpawn - 0.3f);
            currentMaxCars = Mathf.Min(15, currentMaxCars + 1);
            Debug.Log($"Độ khó tăng: spawn {currentMinSpawn:F1}-{currentMaxSpawn:F1}s, tối đa {currentMaxCars} xe");
        }
    }

    void SpawnCar()
    {
        // Kiểm tra mảng npCars
        if (npCars == null || npCars.Length == 0)
        {
            Debug.LogError("Chưa gán npCars trong NPCSpawner!");
            return;
        }

        // Kiểm tra Pooling Instance
        if (NPCPooling.Instance == null)
        {
            Debug.LogError("NPCPooling.Instance null! Hãy thêm PoolManager vào scene.");
            return;
        }

        int carType = Random.Range(0, npCars.Length);
        GameObject car = NPCPooling.Instance.GetCar(carType);
        if (car == null)
        {
            Debug.LogError("GetCar trả về null!");
            return;
        }

        Transform lane = (Random.value < 0.5f) ? laneLeft : laneRight;
        if (lane == null)
        {
            Debug.LogError("laneLeft hoặc laneRight chưa được gán!");
            return;
        }

        float lastCar = GetLastCar(lane);
        float safeDistance = 10f;
        float spawnDistance = float.IsNegativeInfinity(lastCar) ? Random.Range(30f, 70f) : lastCar + safeDistance;

        Vector3 spawnPosition = new Vector3(lane.position.x, lane.position.y, lane.position.z + spawnDistance);
        car.transform.position = spawnPosition;
        car.transform.rotation = lane.rotation;

        DestroyNPC destroy = car.GetComponent<DestroyNPC>();
        if (destroy != null) destroy.spawner = this;

        currentCars++;
    }
    private float GetLastCar(Transform lane)
    {
        float lastCarZ = float.NegativeInfinity;
        NPCMove[] allCars = FindObjectsOfType<NPCMove>();
        foreach (NPCMove car in allCars)
        {
            bool isOnSameLane = Mathf.Abs(car.transform.position.x - lane.position.x) < 0.5f;
            if (isOnSameLane && car.transform.position.z > lastCarZ)
            {
                lastCarZ = car.transform.position.z;
            }
        }
        return lastCarZ;
    }

    public void CarDestroyed()
    {
        currentCars--;
    }
}
