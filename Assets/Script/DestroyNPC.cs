using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyNPC : MonoBehaviour
{
    public NPCSpawner spawner;

    public Transform player;

    public float destroyDistance = 350f;

    void Update()
    {
        if (player == null)
        {
            return;
        }

        float distancePlayer = transform.position.z - player.position.z; //transform.position.z là vị trí của NPC trừ cho vị trí của player 

        NPCMove move = GetComponent<NPCMove>();
            

        bool isOpposite = move != null && move.IsOpposite(); // nếu có NPC di chuyển và là xe đi ngược chiều
            
        if (!isOpposite) // nếu không phải là xe ngược chiều
        {
            // if (distancePlayer > destroyDistance||distancePlayer < -280f)
            if(distancePlayer < -120f)
            {
                ReturnToPool();
            }
        }

        else // nếu là xe ngược chiều
        {
            if (distancePlayer < -120f)
            {
                ReturnToPool();
            }
        }
    }

    void ReturnToPool()
    {
        if (spawner != null)
        {
            spawner.OnCarDestroyed(gameObject);
        }
        else
        {
            NPCPooling.Instance.ReturnCar(gameObject);
        }
    }
}