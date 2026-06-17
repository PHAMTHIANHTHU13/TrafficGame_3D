using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViolationDatabase : MonoBehaviour
{
    public static ViolationDatabase Instance;

    public ViolationInfo[] violations;

    private Dictionary<string, ViolationInfo> violationDict =
        new Dictionary<string, ViolationInfo>();

    void Awake()
    {
        Instance = this;

        foreach (ViolationInfo info in violations)
        {
            violationDict[info.violationName] = info;
        }
    }

    public ViolationInfo GetViolation(string violationName)
    {
        if (violationDict.ContainsKey(violationName))
        {
            return violationDict[violationName];
        }

        return null;
    }
}
