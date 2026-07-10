using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : MonoBehaviour
{

    public float minSpeed = 15f;
    public float maxSpeed = 30f;
    private float currentSpeed;
    private bool isBlocked = false;
    public float detectDistance = 20f;
    public LayerMask npcLayer;

    private TrafficLightController currentLight;

    private bool isOpposite = false;

    private PerformanceTracker tracker;

    void Start()
    {
        tracker = FindObjectOfType<PerformanceTracker>();
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        float difficulty = 0f;

        if (tracker != null)
        {
            difficulty = tracker.difficultyValue;
        }
        float speedMultiplier = Mathf.Lerp(0.7f, 1.5f, difficulty);

        currentSpeed = Random.Range(minSpeed, maxSpeed) * speedMultiplier;

        if (isOpposite)
        {
            currentSpeed *= 1.2f;
        }

        isBlocked = false;

        currentLight = null;
    }

    // SET OPPOSITE

    public void SetOpposite(bool opposite)
    {
        isOpposite = opposite;
    }

    public bool IsOpposite()
    {
        return isOpposite;
    }
    void Update()
    {
        // same direction traffic 

        if (!isOpposite)
        {
            isBlocked = IsBlockedByNPC();
        }
        // opposite traffic
        else
        {
            // opposite traffic
            // ignores front cars
            isBlocked = false;
        }

        // only same-direction cars
        // obey red lights

        bool shouldStop = (!isOpposite && currentLight != null && currentLight.IsRed()) || isBlocked;
        // movement

        float moveSpeed = shouldStop ? 0 : currentSpeed;

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    // NPC BLOCK DETECTION

    bool IsBlockedByNPC()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f + transform.forward, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, detectDistance, npcLayer))
        {
            NPCMove otherCar = hit.collider.GetComponent<NPCMove>();

            if (otherCar != null && otherCar.currentSpeed < currentSpeed * 0.9f)
            {
                return true;
            }
        }

        return false;
    }

    // TRIGGER ENTER

    void OnTriggerEnter(Collider other)
    {
        // road speed limit
        if (other.CompareTag("Road"))
        {
            RoadSegment zone = other.GetComponent<RoadSegment>();
            if (zone != null)
            {
                float difficulty = tracker.difficultyValue;

                float multiplier = Mathf.Lerp(0.7f,1.5f,difficulty);

                currentSpeed =(zone.speedLimit / 5f)* multiplier;

                // opposite traffic slightly faster

                if (isOpposite)
                {
                    currentSpeed *= 1.2f;
                }

                currentSpeed = Mathf.Clamp(currentSpeed, 5f, maxSpeed * 2f);
            }
        }
        // traffic light

        if (!isOpposite)
        {
            TrafficLightController light = other.GetComponent<TrafficLightController>();

            if (light != null)
            {
                currentLight = light;
            }
        }
    }
    // TRIGGER EXIT

    void OnTriggerExit(Collider other)
    {
        TrafficLightController light = other.GetComponent<TrafficLightController>();

        if (light != null && currentLight == light)
        {
            currentLight = null;
        }
    }

    // COLLISION

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            Debug.Log($"{gameObject.name} hit {collision.gameObject.name}");
            // pooling
            NPCPooling.Instance.ReturnCar(gameObject);

            NPCPooling.Instance.ReturnCar(collision.gameObject);

            // remove active list

            DestroyNPC destroyThis = GetComponent<DestroyNPC>();

            DestroyNPC destroyOther = collision.gameObject.GetComponent<DestroyNPC>();

            if (destroyThis != null && destroyThis.spawner != null)
            {
                destroyThis.spawner.OnCarDestroyed(gameObject);
            }

            if (destroyOther != null && destroyOther.spawner != null)
            {
                destroyOther.spawner.OnCarDestroyed(collision.gameObject
                );
            }
        }
    }
}