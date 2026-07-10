using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterController : MonoBehaviour
{
    public float speedMove = 15f;

    public float laneSideWalk = 12f;

    private float currentForwardSpeed; // khai báo biến tốc độ di chuyển hiện tại

    // cooldown phạt khi lên lề
    private float waitTime = 0f; // khai báo thời gian chờ
    public float sidewalkDelay = 2f; // khai báo thời gian nghĩ giữa các lần phạt

    public SpawManager spawManager; // khai báo biến spawManager để quản lý việc tạo đường
                                    // Start is called before the first frame update

    private PerformanceTracker tracker;
    public bool isStopline; // biến để kiểm tra xem có đang ở vạch dừng hay không

    void Start()
    {
        tracker = FindObjectOfType<PerformanceTracker>();
        currentForwardSpeed = speedMove; // gán giá trị tốc độ di chuyển hiện tại bằng giá trị speedMove

    }

    // Update is called once per frame
    void Update()
    {
        UpdateDDASpeed();
        if (Input.GetKey(KeyCode.Space))
        {
            currentForwardSpeed = 0f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            currentForwardSpeed = -speedMove * 0.5f; // nếu nhấn phím mũi tên xuống thì tốc độ di chuyển hiện tại sẽ bằng một nửa giá trị speedMove
        }
        else
        {
            currentForwardSpeed = speedMove; // nếu không nhấn phím Space thì tốc độ di chuyển hiện tại sẽ bằng giá trị speedMove
        }

        float horizontalMove = Input.GetAxis("Horizontal") * speedMove * 0.5f * Time.deltaTime; // chia 2 để nhân vật di chuyển chậm hơn khi di chuyển theo chiều dọc

        float currentX = transform.position.x; // lấy vị trí x hiện tại của nhân vật

        float moveX = currentX + horizontalMove; // tính vị trí x mới sau khi di chuyển

        // giới hạn vị trí x của nhân vật trong phạm vi laneSideWalk
        moveX = Mathf.Clamp(moveX, -laneSideWalk, laneSideWalk); // sử dụng Mathf.Clamp để giới hạn vị trí x của nhân vật trong phạm vi laneSideWalk    

        transform.position = new Vector3(moveX, transform.position.y, transform.position.z);// chỉ di chuyển theo chiều x, giữ nguyên vị trí y và z


        // nhân vật tự di chuyển về phía trước theo tốc độ hiện tại
        transform.Translate(Vector3.forward * currentForwardSpeed * Time.deltaTime); // sử dụng Translate để di chuyển nhân vật về phía trước theo tốc độ hiện tại



        // set thời gian nghĩ 2s sau đó mới dc phạt tiếp tránh gọi liên tục
        if (waitTime > 0f) // nếu thời gian chờ còn lại lớn hơn 0
        {
            waitTime -= Time.deltaTime; // giảm thời gian phạt cho lần tiếp theo
        }

    }

    void UpdateDDASpeed()
    {
        if (tracker == null)
            return;

        //------------------------------------------------
        // DDA player speed
        //------------------------------------------------

        speedMove =
            Mathf.Lerp(
                12f,
                24f,
                tracker.difficultyValue
            );
    }
    public float GetCurrentSpeed()
    {
        return currentForwardSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        spawManager.SpawnTriggerEnterd();



        if (other.CompareTag("Sidewalk") && waitTime <= 0f) //check cái mà player đụng vào có phải là lề không?

        {
            Debug.Log("Bạn đã đi vào vỉa hè!");

            // gọi phương thức DeductPoint từ  lincen để trừ điểm
            ViolationStatistics.Instance.RecordViolation("Đi vào vỉa hè", 4);

            LicensePointManager.Instance.DeductPoint(4);

            UIManager.Instance.showViolationNotyfy("Đi vào vỉa hè", -4);

            if (tracker != null) // điều chỉnh độ khó
            {
                tracker.AddViolation();
            }

            waitTime = sidewalkDelay; // waitTime phải bằng 2f set ở sidewalkDelay thì mới dc phạt tiếp. tránh gọi liên tục
        }


        if (other.CompareTag("NPC"))
        {
            Debug.Log("Va chạm với phương tiện khác");

            ViolationStatistics.Instance.RecordViolation("Va chạm với phương tiện khác", 4);

            LicensePointManager.Instance.DeductPoint(4);

            UIManager.Instance.showViolationNotyfy("Va chạm với phương tiện khác", -4);

            if (tracker != null) // điều chỉnh độ khó
            {
                tracker.AddViolation();
            }


        }

    }
}
