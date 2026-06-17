using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceTracker : MonoBehaviour
{
    //------------------------------------------------
    // REALTIME WINDOW
    //------------------------------------------------

    [Header("Realtime Window")]
    public float evaluationWindow = 30f;

    //------------------------------------------------
    // SURVIVAL
    //------------------------------------------------

    [Header("Realtime Survival")]
    public float recentSurvivalTime;

    //------------------------------------------------
    // VIOLATIONS
    //------------------------------------------------

    [Header("Realtime Violations")]
    public int recentViolationCount;

    //------------------------------------------------
    // DDA RESULT
    //------------------------------------------------

    [Range(0f, 1f)]
    public float difficultyValue;

    //------------------------------------------------
    // violation timestamps
    //------------------------------------------------

    private List<float> violationTimes =
        new List<float>();

    //------------------------------------------------
    // LEVEL
    //------------------------------------------------

    public enum DifficultyLevel
    {
        Beginner,
        Easy,
        Normal,
        Hard,
        Expert
    }

    public DifficultyLevel currentLevel;

    //------------------------------------------------
    // UPDATE
    //------------------------------------------------

    void Update()
    {
        //------------------------------------------------
        // realtime survival
        //------------------------------------------------

        recentSurvivalTime +=
            Time.deltaTime;

        //------------------------------------------------
        // clamp window
        //------------------------------------------------

        if (
            recentSurvivalTime
            > evaluationWindow
        )
        {
            recentSurvivalTime =
                evaluationWindow;
        }

        //------------------------------------------------
        // remove old violations
        //------------------------------------------------

        RemoveExpiredViolations();

        //------------------------------------------------
        // update count
        //------------------------------------------------

        recentViolationCount =
            violationTimes.Count;

        //------------------------------------------------
        // calculate DDA
        //------------------------------------------------

        CalculateDifficulty();
    }

    //------------------------------------------------
    // ADD VIOLATION
    //------------------------------------------------

    public void AddViolation()
    {
        //------------------------------------------------
        // save time
        //------------------------------------------------

        violationTimes.Add(
            Time.time
        );

        //------------------------------------------------
        // giảm difficulty mềm
        //------------------------------------------------

        recentSurvivalTime *= 0.8f;
    }

    //------------------------------------------------
    // REMOVE OLD VIOLATIONS
    //------------------------------------------------

    void RemoveExpiredViolations()
    {
        for (
            int i =
            violationTimes.Count - 1;
            i >= 0;
            i--
        )
        {
            if (
                Time.time
                - violationTimes[i]
                > evaluationWindow
            )
            {
                violationTimes.RemoveAt(i);
            }
        }
    }

    //------------------------------------------------
    // CALCULATE DDA
    //------------------------------------------------

    void CalculateDifficulty()
    {
        //------------------------------------------------
        // survival score
        //------------------------------------------------

        float survivalScore =
            Mathf.Clamp01(
                recentSurvivalTime
                / evaluationWindow
            );

        //------------------------------------------------
        // violation penalty
        //------------------------------------------------

        float violationPenalty =
            Mathf.Clamp01(
                recentViolationCount
                / 5f
            );

        //------------------------------------------------
        // FINAL DDA
        //------------------------------------------------

        difficultyValue =
            survivalScore
            - (violationPenalty * 0.6f);

        difficultyValue =
            Mathf.Clamp01(
                difficultyValue
            );

        //------------------------------------------------
        // classify
        //------------------------------------------------

        if (difficultyValue < 0.2f)
        {
            currentLevel =
                DifficultyLevel.Beginner;
        }
        else if (difficultyValue < 0.4f)
        {
            currentLevel =
                DifficultyLevel.Easy;
        }
        else if (difficultyValue < 0.6f)
        {
            currentLevel =
                DifficultyLevel.Normal;
        }
        else if (difficultyValue < 0.8f)
        {
            currentLevel =
                DifficultyLevel.Hard;
        }
        else
        {
            currentLevel =
                DifficultyLevel.Expert;
        }
    }
}