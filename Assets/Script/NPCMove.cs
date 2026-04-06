using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : MonoBehaviour
{

   public float minSpeed = 12f;
    public float maxSpeed = 20f;
    private float currentSpeed;
    private bool isBlocked = false;
    public float detectDistance = 12f;
    public LayerMask npcLayer;

    private TrafficLightController currentLight;

    void Start()
    {
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        isBlocked = false;
        currentLight = null;
    }

    void Update()
    {
        isBlocked = IsBlockedByNPC();
        bool shouldStop = (currentLight != null && currentLight.IsRed()) || isBlocked;
        float moveSpeed = shouldStop ? 0 : currentSpeed;
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    bool IsBlockedByNPC()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f + transform.forward * 1f, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, detectDistance, npcLayer))
        {
            NPCMove otherCar = hit.collider.GetComponent<NPCMove>();
            if (otherCar != null && otherCar.currentSpeed < currentSpeed * 0.9f)
                return true;
        }
        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Road"))
        {
            RoadSegment zone = other.GetComponent<RoadSegment>();
            if (zone != null)
            {
                currentSpeed = zone.speedLimit / 5f;
                currentSpeed = Mathf.Clamp(currentSpeed, 5f, maxSpeed);
            }
        }
        TrafficLightController light = other.GetComponent<TrafficLightController>();
        if (light != null)
        {
            currentLight = light;
        }
    }

    void OnTriggerExit(Collider other)
    {
        TrafficLightController light = other.GetComponent<TrafficLightController>();
        if (light != null && currentLight == light)
        {
            currentLight = null;
        }
    }
}