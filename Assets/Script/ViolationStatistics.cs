using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViolationStatistics : MonoBehaviour
{
    public static ViolationStatistics Instance;

    private Dictionary<string, int> violationCounts = new Dictionary<string, int>();
    private PerformanceTracker tracker;

    void Awake()
    {
        if (Instance == null)
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
        if (violationCounts.ContainsKey(violationName)) //containskey dùng để kiểm tra tên lỗi vi phạm tôn tại chưa
        {
            violationCounts[violationName]++;
        }
        else
        {
            violationCounts[violationName] = 1;
        }

        tracker?.AddViolation();

        Debug.Log($"Thống kê vi phạm: {violationName}: {violationCounts[violationName]} - {pointDeducted} điểm");
    }

    public string GetStatisticsText()
    {
        if (violationCounts.Count == 0)
        {
            return "Không có lỗi vi phạm";
        }

        string result = "<align=center><color=#b90606><b><size=50>THỐNG KÊ LỖI VI PHẠM</b></color></align>\n";

        foreach (var violation in violationCounts)
        {
            string violationName = violation.Key;
            int count = violation.Value;
             result +=  $"<align=left><color=black><size=42><b>[!] {violationName}: </b></size>" +
                        $"<#b90606><size=42><b>{count} lần</b></size></color></align>\n";
            ViolationInfo info = ViolationDatabase.Instance.GetViolation(violationName);
            if (info != null)
            {
                result += $"<align=left><size=42><b><color=#b90606><b>Theo Nghị định 168/2024/NĐ-CP<b></color> <color=black>:{info.fineMessage}</b></size></color>\n";
                result += $"<align=left><color=#b90606><size=42><b>→ {info.dangerMessage}</b></size></color>\n";
                result += $"<align=left><color=green><size=42><b>=> {info.adviceMessage}</b></size></color>\n";
            }
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
        tracker = FindObjectOfType<PerformanceTracker>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
