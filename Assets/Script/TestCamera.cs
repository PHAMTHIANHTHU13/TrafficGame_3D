using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    private Transform Player; // khơi tạo biến player để theo dõi vị trí của nhân vật
    // transfrom của camera sẽ theo dõi vị trí của player


    // tạo biến yoffset để đặt vị trí của camera so với player
    private float yOffset = 4f;
    // tạo biến zoffset để đặt vị trí của camera so với player
    private float zOffset = -8f;



    // Start is called before the first frame update
    void Start()
    {
        // lấy vị trí transform của player bằng cách tìm kiếm gameobject có tag là player
        Player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // sử dụng hàm lateupdate để camera theo dõi vị trí của player sau khi player đã di chuyển
    void LateUpdate()
    {
        // Đặt lại ví trí cam bằng transform.position của player cộng với một vector3 để đặt cam ở phía sau và cao hơn player
        transform.position = new Vector3(Player.position.x, Player.position.y + yOffset, Player.position.z + zOffset);

        //player.position.x là vị trí chiều ngang của player được giữ nguyên
        //player.position là vị trí của player, cộng với yoffset để đặt cam cao hơn player, 
        // player.position cộng với zoffset để đặt cam ở phía sau player
    }
}
