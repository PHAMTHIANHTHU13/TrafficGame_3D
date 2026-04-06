using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyNPC : MonoBehaviour
{
    public NPCSpawner spawner;
    private float destroyDistance = 45f;

    void Update()
    {
        if (transform.position.z > destroyDistance)
        {
            if (spawner != null)
                spawner.CarDestroyed();
            // Trả xe về pool thay vì Destroy
            NPCPooling.Instance.ReturnCar(gameObject);
        }
    }
}