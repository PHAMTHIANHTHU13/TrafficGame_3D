using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LicensePointManager : MonoBehaviour
{
     public static LicensePointManager Instance;

    public int currentPoint = 12; // khởi tạo điểm mặc định là 12

    void Awake() 
    {
        Instance = this;
    }

    public void DeductPoint(int amount)
    {
        currentPoint -= amount;

        // Debug.Log(" Bị trừ " + amount + " điểm. Còn lại: " + currentPoint);
        Debug.Log($"{amount} điểm còn lại {currentPoint}");

        if (currentPoint <= 0)
        {
            Debug.Log("Trò chơi kết thúc!");
            Time.timeScale = 0f; // dừng trò chơi khi hết điểm
        }
    }
}
