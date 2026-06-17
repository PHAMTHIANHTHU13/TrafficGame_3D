using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawManager : MonoBehaviour
{
    

    // RoadSpawner roadSpawner; // khai báo biến roadspawner để quản lý việc tạo đường
    // PlotSpawner plotSpawner; // khai báo biến plotspawner để quản lý việc tạo đất

     void Awake()
    {
        // roadSpawner = GetComponent<RoadSpawner>(); // lấy component RoadSpawner từ game object này
        // plotSpawner = GetComponent<PlotSpawner>(); // lấy component PlotSpawner từ game object này
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // gọi phương thức spawTrigger 
    public void SpawnTriggerEnterd()
    {
        // roadSpawner.SpawnRoad(); // gọi phương thức SpawnRoad từ roadSpawner để tạo đường mới khi trigger được kích hoạt
        // plotSpawner.SpawnPlot(); // gọi phương thức SpawnPlot từ plotSpawner để tạo đất mới khi trigger được kích hoạt
    }
}
