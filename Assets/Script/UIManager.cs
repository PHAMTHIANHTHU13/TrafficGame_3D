using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI pointText;
    public TextMeshProUGUI notifyText;

    private Coroutine notifyCoroutine;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; //this là instance của UIManager, gán nó cho Instance để có thể truy cập từ các script khác
        }
        else
        {
            Destroy(gameObject);
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        int currentPoint = LicensePointManager.Instance.currentPoint;

        UpdatePoint(currentPoint);

        if (notifyText != null)
        {
            notifyText.gameObject.SetActive(false); // ẩn thông báo khi bắt đầu game
        }
    }

    public void UpdatePoint(int currentPoint)
    {
        if (pointText != null)
        {
            pointText.text = $"Điểm GPLX : {currentPoint}";
        }
    }

    public void showViolationNotyfy(string violationName, int pointDeducted)
    {
        if (notifyText == null)
        {
            return;
        }

        ViolationInfo info = ViolationDatabase.Instance.GetViolation(violationName);


        string message = $"{violationName} {pointDeducted} điểm";


        if (info != null)
        {
            message += $" || {info.NotifyName}";

        }

        if (notifyCoroutine != null)
        {
            StopCoroutine(notifyCoroutine);
        }
        notifyCoroutine = StartCoroutine(showNotifyfuction(message));
    }


    IEnumerator showNotifyfuction(string message)
    {
        notifyText.text = message;
        notifyText.gameObject.SetActive(true);

        // yield return new WaitForSeconds(1f);
        yield return new WaitForSecondsRealtime(1f);

        notifyText.gameObject.SetActive(false); // dùng để ẩn thông báo sau khi hiển thị trong 2 giây

        notifyCoroutine = null;// này dùng để đặt lại notify để thông báo lỗi vi phạm mới

    }

    public void HideGameplayUI()
    {
        if (pointText != null)
        {
            pointText.gameObject.SetActive(false);
        }

        if (notifyText != null)
        {
            notifyText.gameObject.SetActive(false);
        }

        if (notifyCoroutine != null)
        {
            StopCoroutine(notifyCoroutine);

            notifyCoroutine = null;
        }
    }



    // Update is called once per frame
    void Update()
    {

    }
}
