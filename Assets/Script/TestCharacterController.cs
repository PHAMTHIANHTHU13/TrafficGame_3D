using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterController : MonoBehaviour
{
    public float speedMove = 10f;

    public float laneSideWalk = 12f;

    private float currentForwardSpeed; // khai báo biến tốc độ di chuyển hiện tại

    // cooldown phạt khi lên lề
    private float waitTime = 0f; // khai báo thời gian chờ
    public float sidewalkDelay = 2f; // khai báo thời gian nghĩ giữa các lần phạt

    public SpawManager spawManager; // khai báo biến spawManager để quản lý việc tạo đường
                                    // Start is called before the first frame update


    void Start()
    {
        currentForwardSpeed = speedMove; // gán giá trị tốc độ di chuyển hiện tại bằng giá trị speedMove
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            currentForwardSpeed = 0f;
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
    public float GetCurrentSpeed()
    {
        return currentForwardSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        spawManager.SpawnTriggerEnterd();


        // gọi sự kiện khi player đi vào vỉa hè
        if (other.CompareTag("Sidewalk") && waitTime <= 0f) //check cái mà player đụng vào có phải là lề không?

        {
            Debug.Log("Bạn đã đi vào vỉa hè!");

            // gọi phương thức DeductPoint từ  lincen để trừ điểm
            LicensePointManager.Instance.DeductPoint(4);

            waitTime = sidewalkDelay; // waitTime phải bằng 2f set ở sidewalkDelay thì mới dc phạt tiếp. tránh gọi liên tục
        }



    }
}
