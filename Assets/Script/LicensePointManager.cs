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

    public void DeductPoint(int pointDeduct)
    {
        currentPoint -= pointDeduct;
        UIManager.Instance.UpdatePoint(currentPoint); // Hiển thị điểm ui

        // Debug.Log(" Bị trừ " + amount + " điểm. Còn lại: " + currentPoint);
        Debug.Log($"- {pointDeduct} điểm còn lại {currentPoint}");

        if (currentPoint <= 0) // 
        {
            currentPoint = 0;
            Debug.Log("Trò chơi kết thúc!");
            Debug.Log("GameOver.Instance: " + GameOver.Instance);
            GameOver.Instance.ShowGameOver(); // hiển thị giao diện gameover

            Time.timeScale = 0f; // dừng trò chơi khi hết
        }
    }
}
