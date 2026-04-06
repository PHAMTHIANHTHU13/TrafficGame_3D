using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    // tạo danh sách công khai cho các prefab đường
    public List<GameObject> roads; // List này dùng để hiển thị các prefab đường trong gameobj road

    
    private float offset = 40; // chiều dài của mỗi đoạn đường là 80

    public int startRoads = 5; // khi game bắt đầu sẽ tạo ra 5 đoạn đường

    private float nextSpawnZ = 0f; // tọa độ z để spawn đoạn đường tiếp theo, ban đầu là 0
    // kiểm tra sự trùng lặp random
    private int lastRandomIndex = -1; // biến để lưu chỉ số của đường đã được spawn gần nhất

    public Transform player;        // vị trí player
    public float spawnDistance = 120f; // kiểm tra khoảng cách phía trước player để spawn road mới

    public bool isPlayerLive = true; // biến để kiểm tra xem người chơi còn sống hay không

    public float currentSpeedLimit = 70f; // tốc độ hiện tại của player, mặc định là 70


    // Start is called before the first frame update
    void Start()
    {
        for (int i =0 ; i < startRoads; i++) //vòng lặp tạo ra 5 lần spawn road mới

        {
            SpawnRoad();  // gọi phương thức spawnRoad để tạo đường mới
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerLive)
        {
            if (player.position.z + spawnDistance > nextSpawnZ)
            {
                SpawnRoad();
            }
        }
    }

    // gọi phương thức spawnRoad để tạo đường mới
    public void SpawnRoad()
    {
        
        int randomList = Random.Range(0, roads.Count); // tạo một số ngẫu nhiên để chọn một prefab đường từ danh sách roads
        if(randomList == lastRandomIndex)// nếu danh sách đường random trùng với đường đã spawn gần nhất thì random lại
        {
            randomList = Random.Range(0,roads.Count);// random đường từ 0 đến đến roads.Count. trong đó 
            //roads.Count là số lượng đường có trong danh sách roads
        }
        lastRandomIndex = randomList; // lưu prefab đường vừa spawn vào lastRandomIndex để kiểm tra trùng lặp trong lần spawn tiếp theo



        GameObject newRoad = Instantiate(roads[randomList], transform.forward * nextSpawnZ, Quaternion.identity); //tạo một đoạn đường mới bằng cách sử dụng prefab đã chọn từ danh sách roads, vị trí spawn được xác định bởi transform.forward * nextSpawnZ,
        //  và không có sự xoay (Quaternion.identity)

        RoadSegment roadSegment = newRoad.GetComponent<RoadSegment>();// lấy component RoadSegment từ prefab đường đã chọn để kiểm tra xem đoạn đường có biển báo tốc độ hay không
            if(roadSegment.hasSpeedSign) // nếu đoạn đường có biển báo tốc độ
            {
                currentSpeedLimit = roadSegment.speedLimit; // cập nhật tốc độ hiện tại của player bằng giới hạn tốc độ của đoạn đường
            }
            
            roadSegment.speedLimit = currentSpeedLimit; // truyền tốc độ hiện tại của player vào component RoadSegment để các đoạn đường sau có thể kế thừa tốc độ này
        nextSpawnZ += offset; // mỗi lần tăng offset 40 để tạo ra đoạn đường tiếp theo ở vị trí z.
    }
}
