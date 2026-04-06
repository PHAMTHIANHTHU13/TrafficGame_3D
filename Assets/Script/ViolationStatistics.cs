using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViolationStatistics : MonoBehaviour
{
    public static ViolationStatistics Instance;

    private Dictionary<string, int> violationCounts = new Dictionary<string, int>();

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RecordViolation(string violationName, int pointDeducted)
    {
        if(violationCounts.ContainsKey(violationName)) //containskey dùng để kiểm tra tên lỗi vi phạm tôn tại chưa
        {
            violationCounts[violationName] ++;
        }
        else
        {
            violationCounts[violationName] =1;
        }

        Debug.Log($"Thống kê vi phạm: {violationName}: {violationCounts[violationName]} - {pointDeducted} điểm");
    }

    public string GetStatisticsText()
    {
        if(violationCounts.Count == 0)
        {
            return "không có lỗi vi phạm";
        }

        string result = "BẢNG THỐNG KÊ LỖI VI PHẠM\n";

        foreach(var violation in violationCounts)
        {
            result += $"{violation.Key}: {violation.Value} lần\n";
        }

        return result; 
    }

    public void ResetStatistics() //
    {
        violationCounts.Clear(); //clear dùng để xóa hêt dữ liệu
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
