using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadReturn : MonoBehaviour
{
    public Transform player;
    public float disableDistance = 120f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         if (!player) return;

        if (transform.position.z < player.position.z - disableDistance)
        {
            Debug.Log("Return road: " + gameObject.name);
            gameObject.SetActive(false);
        }
    }
}
