using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceTracker : MonoBehaviour
{
    public float evaluationWindow = 30f;
    public float recentSurvivalTime;

    public int recentViolationCount;

    public float difficultyValue;

    private List<float> violationTimes = new List<float>();

    public enum DifficultyLevel
    {
        Beginner,
        Easy,
        Normal,
        Hard,
        Expert
    }

    public DifficultyLevel currentLevel;

    void Update()
    {
        recentSurvivalTime += Time.deltaTime;
        if (recentSurvivalTime > evaluationWindow)
        {
            recentSurvivalTime = evaluationWindow;
        }

        RemoveExpiredViolations();

        recentViolationCount = violationTimes.Count;

        CalculateDifficulty();
    }
    public void AddViolation()
    {
        violationTimes.Add(Time.time);
        // recentSurvivalTime *= 0.5f;
        recentSurvivalTime = Mathf.Max(0, recentSurvivalTime - 5f);
    }

    void RemoveExpiredViolations()
    {
        for (int i = violationTimes.Count - 1; i >= 0; i--)
        {
            if (Time.time - violationTimes[i] > evaluationWindow)
            {
                violationTimes.RemoveAt(i);
            }
        }
    }

    void CalculateDifficulty()
    {
        float survivalScore = Mathf.Clamp01(recentSurvivalTime / evaluationWindow);

        float violationPenalty = Mathf.Clamp01(recentViolationCount / 5f);

        difficultyValue = survivalScore - (violationPenalty * 0.6f);

        difficultyValue = Mathf.Clamp01(difficultyValue);

        if (difficultyValue < 0.2f)
        {
            currentLevel = DifficultyLevel.Beginner;
        }
        else if (difficultyValue < 0.4f)
        {
            currentLevel = DifficultyLevel.Easy;
        }
        else if (difficultyValue < 0.6f)
        {
            currentLevel = DifficultyLevel.Normal;
        }
        else if (difficultyValue < 0.8f)
        {
            currentLevel = DifficultyLevel.Hard;
        }
        else
        {
            currentLevel = DifficultyLevel.Expert;
        }

        Debug.Log("Difficulty: "+ difficultyValue+ " | Violations: " + recentViolationCount); // log độ khó và số vi phạm để kiểm tra
    }
}