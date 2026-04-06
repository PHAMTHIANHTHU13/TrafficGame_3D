using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Performance : MonoBehaviour
{
    [Header("Tham chiếu đến các script khác")]
    public LicensePointManager licenseManager;  // Kéo LicensePointManager vào đây
    public TestCharacterController playerController; // Kéo nhân vật Player vào đây

    [Header("Các chỉ số theo dõi (chỉ đọc, để debug)")]
    [SerializeField] private float survivalTime = 0f;      // Tổng thời gian chơi (giây)
    [SerializeField] private int recentViolations = 0;     // Số lần vi phạm trong 10 giây qua
    [SerializeField] private float lastViolationTime = 0f; // Thời điểm vi phạm gần nhất

    [Header("Cài đặt DDA")]
    [Range(0f, 1f)] public float currentSkillScore = 0.5f; // 0 = rất yếu, 1 = rất giỏi. Khởi tạo ở mức trung bình.
    public float evaluationInterval = 10f;  // Cứ mỗi 10 giây đánh giá lại một lần

    // Biến private để đếm thời gian
    private float evaluationTimer = 0f;
    private float violationTimer = 0f;

    void Start()
    {
        // Tự động tìm nếu chưa gán
        if (licenseManager == null)
            licenseManager = FindObjectOfType<LicensePointManager>();
        if (playerController == null)
            playerController = FindObjectOfType<TestCharacterController>();

        ResetViolationCount();
    }

    void Update()
    {
        // 1. Cập nhật thời gian sống sót
        survivalTime += Time.deltaTime;

        // 2. Tự động reset số lần vi phạm sau mỗi 10 giây
        violationTimer += Time.deltaTime;
        if (violationTimer >= 10f)
        {
            ResetViolationCount();
        }

        // 3. Cứ mỗi evaluationInterval giây, tính lại skillScore
        evaluationTimer += Time.deltaTime;
        if (evaluationTimer >= evaluationInterval)
        {
            CalculateSkillScore();
            evaluationTimer = 0f;
        }
    }

    /// <summary>
    /// Hàm này được gọi từ LicensePointManager mỗi khi người chơi bị trừ điểm (vi phạm)
    /// </summary>
    public void OnViolationOccurred(int pointsDeducted)
    {
        recentViolations++;
        lastViolationTime = Time.time;
        Debug.Log($"[DDA] Vi phạm! Số lần vi phạm gần đây: {recentViolations}");
    }

    /// <summary>
    /// Reset số lần vi phạm sau mỗi 10 giây
    /// </summary>
    void ResetViolationCount()
    {
        recentViolations = 0;
        violationTimer = 0f;
    }

    /// <summary>
    /// Tính toán điểm kỹ năng dựa trên các yếu tố
    /// Công thức: skillScore = (licenseScore * 0.6) + (survivalScore * 0.2) + (violationScore * 0.2)
    /// </summary>
    void CalculateSkillScore()
    {
        // --- Yếu tố 1: Điểm bằng lái còn lại (quan trọng nhất, 60%) ---
        float licenseScore = 0f;
        if (licenseManager != null)
        {
            // Giả sử điểm tối đa là 12
            int maxPoints = 12;
            licenseScore = (float)licenseManager.currentPoint / maxPoints;
        }

        // --- Yếu tố 2: Thời gian sống sót (càng lâu càng tốt, 20%) ---
        // Giả sử mục tiêu là sống 60 giây là "xuất sắc"
        float survivalScore = Mathf.Clamp01(survivalTime / 60f);

        // --- Yếu tố 3: Số lần vi phạm gần đây (càng ít càng tốt, 20%) ---
        // Giả sử 5 lần vi phạm trong 10 giây là "rất tệ"
        float violationScore = 1f - Mathf.Clamp01(recentViolations / 5f);

        // Kết hợp với trọng số
        float newSkillScore = (licenseScore * 0.6f) + (survivalScore * 0.2f) + (violationScore * 0.2f);
        currentSkillScore = Mathf.Clamp01(newSkillScore);

        // In ra Console để debug (giúp bạn hiểu chuyện gì đang xảy ra)
        Debug.Log($"=== DDA UPDATE === SkillScore: {currentSkillScore:F2} | License: {licenseScore:F2} | Survival: {survivalScore:F2} | Violations: {recentViolations}");
    }

    /// <summary>
    /// Lấy điểm kỹ năng hiện tại (các script khác sẽ gọi hàm này)
    /// </summary>
    public float GetCurrentSkillScore()
    {
        return currentSkillScore;
    }
}
