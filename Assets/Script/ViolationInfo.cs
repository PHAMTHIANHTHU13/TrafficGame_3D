using UnityEngine;

[System.Serializable]
public class ViolationInfo
{
    public string violationName;

    // mức phạt tiền
    public string NotifyName;
    [TextArea]

    public string fineMessage;

    // cảnh báo nguy hiểm
    [TextArea]
    public string dangerMessage;

    // lời khuyên giáo dục
    [TextArea]
    public string adviceMessage;
}