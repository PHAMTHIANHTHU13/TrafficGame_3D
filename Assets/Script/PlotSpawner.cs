using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotSpawner : MonoBehaviour
{
    private int initAmount = 5; // số lượng plot ban đầu sẽ được spawn
   
    private float plotLength = 40f; // chiều dài của mỗi plot là 40
    private float xplotLeft= -14f; // tọa độ x để spawn plot bên trái
    private float xplotRight= 14f; // tọa độ x để spawn plot bên phải

    private float lastZpos= 0f; // biến để lưu tọa độ z của plot đã được spawn gần nhất

    public Transform player; // kiểm tra vị trí của player để pawn plot

    public float spawnDistance = 120f; // kiểm tra khoảng cách trước player để spawn plot mới

    public List<GameObject> plots; // danh sách các prefab plot để spawn

    // quản lí pbj plot đã được spawn
    public List<GameObject> spawnedPlots = new List<GameObject>(); // danh sách để lưu các plot đã được spawn
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < initAmount; i++) // vòng lặp để spawn plot ban đầu
        {
            SpawnPlot(); // gọi phương thức SpawnPlot 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(player.position.z + spawnDistance > lastZpos) // kiểm tra nếu vị trí z của player cộng với khoảng cách spawn lớn hơn tọa độ z của plot đã spawn gần nhất
        {
            SpawnPlot(); // gọi phương thức SpawnPlot để spawn plot mới
        }
    }

    public void SpawnPlot()
    {
        // random prfab plot trái và phải từ danh sách plots 
        GameObject plotLeft = plots[Random.Range(0, plots.Count)];
        GameObject plotRight = plots[Random.Range(0,plots.Count)];

        //pawn plot trái và phải xplotLeft và xplotRight với cùng một tọa độ z là lastZpos
        GameObject left = Instantiate(plotLeft, new Vector3(xplotLeft,0, lastZpos), Quaternion.identity);//quaternion.identity là không tự xoay plot khi spawn
        // xplotLeft và lastZpó là tọa độ dc khai báo trên;

        GameObject right = Instantiate(plotRight, new Vector3(xplotRight, 0, lastZpos), Quaternion.Euler(0,180,0)); // Quaternion.Euler(0,180,0) để xoay plot phải 180 độ quanh trục y để nó hướng về phía player

        //
        spawnedPlots.Add(left); // thêm plot trái vào danh sách plot đã spawn
        spawnedPlots.Add(right); // thêm plot phải vào danh sách plot đã spawn


        //nếu quá 10 obj plot đã spawn thì destroy plot đã spawn sau đó spawn plot mới
        if(spawnedPlots.Count>20)
        {
            // destroy plot trái và phải đã spawn đầu tiên trong danh sách spawnedPlots
            Destroy(spawnedPlots[0]); // 0 là index của plot trái đã spawn đầu tiên trong danh sách spawnedPlots
            Destroy(spawnedPlots[1]); // 1 là index của plot phải đã spawn đầu tiên trong danh sách spawnedPlots

            // sau khi destroy plot đã spawn đầu tiên thì xóa chúng khỏi danh sách spawnedPlots
            spawnedPlots.RemoveAt(0);  // 0 là plot trái đầu tiên
            spawnedPlots.RemoveAt(0);  // 0 là sau khi xóa plot trái đầu tiên thì plot phải thành 0
        }

        //tăng vị trí z của plot mới bằng cách cộng thêm chiều dài của plot củ.
        lastZpos += plotLength; // laszpos = 50 + 40 plotlength = 90 thì vị trí tiếp theo của plot sẽ là 90
    }

    
}
