using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPooling : MonoBehaviour
{
    public static NPCPooling Instance;
    public GameObject[] carPrefabs;  // Kéo các prefab xe vào đây
    public int poolSize = 10;

    private List<GameObject>[] pools;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitPools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitPools()
    {
        pools = new List<GameObject>[carPrefabs.Length];
        for (int i = 0; i < carPrefabs.Length; i++)
        {
            pools[i] = new List<GameObject>();
            for (int j = 0; j < poolSize; j++)
            {
                GameObject obj = Instantiate(carPrefabs[i]);
                obj.SetActive(false);
                pools[i].Add(obj);
            }
        }
    }

    public GameObject GetCar(int typeIndex)
    {
        if (typeIndex >= pools.Length) typeIndex = 0;
        foreach (GameObject car in pools[typeIndex])
        {
            if (!car.activeInHierarchy)
            {
                car.SetActive(true);
                // Reset vị trí, trạng thái nếu cần
                NPCMove move = car.GetComponent<NPCMove>();
                if (move != null) move.ResetToDefault();
                return car;
            }
        }
        // Mở rộng pool
        GameObject newCar = Instantiate(carPrefabs[typeIndex]);
        pools[typeIndex].Add(newCar);
        return newCar;
    }

    public void ReturnCar(GameObject car)
    {
        car.SetActive(false);
        car.transform.position = Vector3.zero;
    }
}