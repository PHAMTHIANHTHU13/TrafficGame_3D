using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject[] npCars;
    public Transform laneLeft;
    public Transform laneRight;
    public Transform laneLeftOpposite;
    public Transform laneRightOpposite;
    public Transform player;
    public PerformanceTracker tracker;
    public int easyCarCount = 4;
    public int hardCarCount = 8;
    public float easyOppositeChance = 0.1f;
    public float hardOppositeChance = 0.45f;
    public float easyTrafficDistance = 220f;
    public float hardTrafficDistance = 120f;
    public int maxCarsPerLane = 2;
    public float minSpawnGap = 35f;
    public float sideBySideBlockDistance = 20f;
    public float escapeGapDistance = 45f; // khoảng cách sinh các phương tiện đi ngược chiều
    public float safeLaneChance = 0.35f;
    private int targetCarCount;
    private float currentOppositeChance;
    private float currentTrafficDistance;

    private List<GameObject> activeCars = new List<GameObject>();

    private TestCharacterController playerController;

    void Start()
    {
        if (tracker == null)
        {
            tracker = FindObjectOfType<PerformanceTracker>();
        }

        playerController = player.GetComponent<TestCharacterController>();

        UpdateDDAValues();
        FillTraffic();
    }
    void Update()
    {
        if (tracker == null || player == null)
        {
            return;
        }

        UpdateDDAValues();

        activeCars.RemoveAll(car => car == null || !car.activeInHierarchy);

        FillTraffic();
    }

    void UpdateDDAValues() //t dùng để nội suy giữa giá trị dễ và khó dựa trên độ khó hiện tại từ tracker để điều chỉnh số lượng xe, khoảng cách sinh xe và xác suất sinh xe ngược chiều.
    {
        float t = tracker.difficultyValue;

        targetCarCount = Mathf.RoundToInt(Mathf.Lerp(easyCarCount, hardCarCount, t)); // lấy độ khó từ 0-1 từ tracker

        currentTrafficDistance = Mathf.Lerp(easyTrafficDistance, hardTrafficDistance, t);

        if (t < 0.5f)
        {
            currentOppositeChance = 0f;
        }
        else
        {
            float normalizedT = Mathf.InverseLerp(0.5f, 1f, t); // nor
            currentOppositeChance = Mathf.Lerp(0.05f, hardOppositeChance, normalizedT);
        }
    }
    void FillTraffic()
    {
        int frontCars = 0;

        foreach (GameObject car
            in activeCars)
        {
            if (
                car == null
                || !car.activeInHierarchy
            )
                continue;

            float deltaZ =
                car.transform.position.z
                - player.position.z;

            if (
                deltaZ > 0
                &&
                deltaZ < currentTrafficDistance
            )
            {
                frontCars++;
            }
        }

        //------------------------------------------------
        // random density
        //------------------------------------------------

        int desiredCars =
            Random.Range(
                targetCarCount - 1,
                targetCarCount + 1
            );

        desiredCars =
            Mathf.Max(2, desiredCars);

        //------------------------------------------------
        // spawn thêm nếu thiếu
        //------------------------------------------------

        int safetyLoop = 20;

        while (
            frontCars < desiredCars
            &&
            safetyLoop > 0
        )
        {
            bool spawned =
                SpawnSingleCar();

            if (spawned)
            {
                frontCars++;
            }

            safetyLoop--;
        }
    }

    //------------------------------------------------
    // SPAWN SINGLE CAR
    //------------------------------------------------

    bool SpawnSingleCar()
    {
        //------------------------------------------------
        // safe lane
        //------------------------------------------------

        bool forceSafeLane =
            Random.value
            < safeLaneChance;

        //------------------------------------------------
        // opposite?
        //------------------------------------------------

        bool isOpposite =
            Random.value
            < currentOppositeChance;

        //------------------------------------------------
        // choose lane
        //------------------------------------------------

        if(playerController.isStopline) // nếu player đang dừng thì không sinh ptieenj đi ngược chiều
        {
            isOpposite = false;
        }

        Transform lane =
            ChooseLane(
                isOpposite,
                forceSafeLane
            );

        //------------------------------------------------
        // invalid lane
        //------------------------------------------------

        if (lane == null)
            return false;

        //------------------------------------------------
        // limit lane density
        //------------------------------------------------

        if (
            CountCarsInLane(lane)
            >= maxCarsPerLane
        )
        {
            return false;
        }

        //------------------------------------------------
        // get pooled car
        //------------------------------------------------

        int carType =
            Random.Range(
                0,
                npCars.Length
            );

        GameObject car =
            NPCPooling.Instance
            .GetCar(carType);

        //------------------------------------------------
        // spawn distance
        //------------------------------------------------

        float minDistance =
            isOpposite ? 160f : 70f;

        float maxDistance =
            isOpposite
            ? currentTrafficDistance + 60f
            : currentTrafficDistance;

        float spawnZ =
            player.position.z
            + Random.Range(
                minDistance,
                maxDistance
            );

        //------------------------------------------------
        // prevent impossible opposite wall
        //------------------------------------------------

        if (
            IsEscapeBlocked(
                isOpposite,
                spawnZ
            )
        )
        {
            NPCPooling.Instance
                .ReturnCar(car);

            return false;
        }

        //------------------------------------------------
        // prevent side by side wall
        //------------------------------------------------

        if (
            IsSideBySideBlocked(
                spawnZ
            )
        )
        {
            NPCPooling.Instance
                .ReturnCar(car);

            return false;
        }

        //------------------------------------------------
        // occupied?
        //------------------------------------------------

        if (
            IsLaneOccupied(
                lane,
                spawnZ,
                minSpawnGap
            )
        )
        {
            NPCPooling.Instance
                .ReturnCar(car);

            return false;
        }

        //------------------------------------------------
        // set transform
        //------------------------------------------------

        car.transform.position =
            new Vector3(
                lane.position.x,
                lane.position.y,
                spawnZ
            );

        car.transform.rotation =
            lane.rotation;

        //------------------------------------------------
        // movement
        //------------------------------------------------

        NPCMove move =
            car.GetComponent<NPCMove>();

        if (move != null)
        {
            move.SetOpposite(
                isOpposite
            );

            move.ResetToDefault();
        }

        //------------------------------------------------
        // destroy npc
        //------------------------------------------------

        DestroyNPC destroy =
            car.GetComponent
            <DestroyNPC>();

        if (destroy != null)
        {
            destroy.spawner = this;
            destroy.player = player;
        }

        //------------------------------------------------
        // add list
        //------------------------------------------------

        if (!activeCars.Contains(car))
        {
            activeCars.Add(car);
        }

        return true;
    }



    //------------------------------------------------
    // CHOOSE LANE
    //------------------------------------------------

    Transform ChooseLane(
        bool isOpposite,
        bool preferSafe
    )
    {
        List<Transform> lanes =
            new List<Transform>();

        if (isOpposite)
        {
            lanes.Add(
                laneLeftOpposite
            );

            lanes.Add(
                laneRightOpposite
            );
        }
        else
        {
            lanes.Add(laneLeft);
            lanes.Add(laneRight);
        }

        //------------------------------------------------
        // tìm lane ít xe nhất
        //------------------------------------------------

        Transform bestLane = null;

        int lowestCount =
            int.MaxValue;

        foreach (Transform lane
            in lanes)
        {
            int count =
                CountCarsInLane(
                    lane
                );

            if (count < lowestCount)
            {
                lowestCount =
                    count;

                bestLane = lane;
            }
        }

        //------------------------------------------------
        // safe lane system
        //------------------------------------------------

        if (
            preferSafe
            &&
            lowestCount >= 1
        )
        {
            return null;
        }

        return bestLane;
    }

    //------------------------------------------------
    // COUNT CARS IN LANE
    //------------------------------------------------

    int CountCarsInLane(
        Transform lane
    )
    {
        int count = 0;

        foreach (GameObject car
            in activeCars)
        {
            if (
                car == null
                || !car.activeInHierarchy
            )
                continue;

            bool sameLane =
                Mathf.Abs(
                    car.transform.position.x
                    - lane.position.x
                ) < 0.5f;

            if (!sameLane)
                continue;

            float deltaZ =
                car.transform.position.z
                - player.position.z;

            if (
                deltaZ > 0
                &&
                deltaZ < currentTrafficDistance
            )
            {
                count++;
            }
        }

        return count;
    }

    //------------------------------------------------
    // IS LANE OCCUPIED
    //------------------------------------------------

    bool IsLaneOccupied(
        Transform lane,
        float z,
        float range
    )
    {
        foreach (GameObject car
            in activeCars)
        {
            if (
                car == null
                || !car.activeInHierarchy
            )
                continue;

            bool sameLane =
                Mathf.Abs(
                    car.transform.position.x
                    - lane.position.x
                ) < 0.5f;

            if (!sameLane)
                continue;

            float distance =
                Mathf.Abs(
                    car.transform.position.z
                    - z
                );

            if (distance < range)
            {
                return true;
            }
        }

        return false;
    }

    //------------------------------------------------
    // SIDE BY SIDE CHECK
    //------------------------------------------------

    bool IsSideBySideBlocked(
        float spawnZ
    )
    {
        foreach (GameObject car
            in activeCars)
        {
            if (
                car == null
                || !car.activeInHierarchy
            )
                continue;

            float distance =
                Mathf.Abs(
                    car.transform.position.z
                    - spawnZ
                );

            if (
                distance
                < sideBySideBlockDistance
            )
            {
                return true;
            }
        }

        return false;
    }

    bool IsEscapeBlocked(
    bool isOpposite,
    float spawnZ
)
    {
        //------------------------------------------------
        // chỉ kiểm tra xe ngược chiều
        //------------------------------------------------

        if (!isOpposite)
            return false;

        int oppositeCount = 0;

        foreach (GameObject car
            in activeCars)
        {
            if (
                car == null
                || !car.activeInHierarchy
            )
                continue;

            NPCMove move =
                car.GetComponent<NPCMove>();

            if (move == null)
                continue;

            //------------------------------------------------
            // chỉ xét xe ngược chiều
            //------------------------------------------------

            bool otherOpposite =
                car.transform.forward.z < 0;

            if (!otherOpposite)
                continue;

            //------------------------------------------------
            // gần player
            //------------------------------------------------

            float distance =
                Mathf.Abs(
                    car.transform.position.z
                    - spawnZ
                );

            if (
                distance
                < escapeGapDistance
            )
            {
                oppositeCount++;
            }
        }

        //------------------------------------------------
        // nếu đã có opposite traffic
        // thì không spawn thêm
        //------------------------------------------------

        return oppositeCount >= 1;
    }

    //------------------------------------------------
    // REMOVE CAR
    //------------------------------------------------

    public void OnCarDestroyed(GameObject car)
    {
        if (activeCars.Contains(car))
        {
            activeCars.Remove(car);
        }

        NPCPooling.Instance.ReturnCar(car);
    }
}

