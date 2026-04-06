using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopLineChecker : MonoBehaviour
{
    public TrafficLightController trafficLight; 


    public float stopSpeed = 0.5f; 
                                   

    private void OnTriggerEnter(Collider other) 
    {
        // lấy tốc độ của player
        TestCharacterController playerController = other.GetComponentInParent<TestCharacterController>(); // lấy TestCharacterController của player để kiểm tra tốc độ, dùng GetComponentInParent vì collider có thể nằm ở con của player
  
        if (!other.CompareTag("Player")) return; // kiểm tra xem cái mà đụng vào stopline có phải là player không ?

        float speedPlayer = playerController.GetCurrentSpeed(); // lấy tốc độ của player 

        if (trafficLight.IsRed())
        {
            if (speedPlayer > stopSpeed) // nếu tốc độ player lớn hơn tốc độ dừng là 0.5
            {
                Debug.Log("Bạn đã vượt đèn đỏ!");

                LicensePointManager.Instance.DeductPoint(4);
            }

        }


        
    }
}
