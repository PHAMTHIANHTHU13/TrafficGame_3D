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

        if (!other.CompareTag("Player")) // kiểm tra xem cái mà đụng vào stopline có phải là player không ?
        {
            return;
        }

        if (playerController != null) // kiểm tra xem player có đang dừng đèn đỏ không 
        {
            playerController.isStopline = true;
        }

        float speedPlayer = playerController.GetCurrentSpeed(); // lấy tốc độ của player 

        if (trafficLight.IsRed())
        {
            if (speedPlayer > stopSpeed) // nếu tốc độ player lớn hơn tốc độ dừng là 0.5
            {
                Debug.Log("Bạn đã vượt đèn đỏ!");

                ViolationStatistics.Instance.RecordViolation("Vượt đèn đỏ", -4);

                LicensePointManager.Instance.DeductPoint(4);

                UIManager.Instance.showViolationNotyfy("Vượt đèn đỏ", -4);
                // điều chỉnh độ khó
                PerformanceTracker tracker =FindObjectOfType<PerformanceTracker>();

                if (tracker != null)
                {
                    tracker.AddViolation();
                }

            }

        }

    }

    private void OnTriggerExit(Collider other) // khi player rời khỏi vạch dừng
    {
        TestCharacterController playerController = other.GetComponentInParent<TestCharacterController>();

        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (playerController != null)
        {
            playerController.isStopline = false; // khi rời khỏi vạch dừng thì isStopline sẽ là false
        }
    }
}
