using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject wallCornerPrefab;
    public GameObject wallBrokePrefab;
    public GameObject wallDoorPrefab;
    public GameObject doorPrefab;


    private int FLOOR_FACTOR = 6;
    private int WALL_FACTOR = 2;
    private int MIN_SIZE = 1;
    private int MAX_SIZE = 3;

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        GenerateRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateRoom(){
        int sizeZ = Random.Range(MIN_SIZE, MAX_SIZE) * FLOOR_FACTOR;
        int sizeX = Random.Range(MIN_SIZE, MAX_SIZE) * FLOOR_FACTOR;
        CreateFloor(sizeZ, sizeX);
        CreateAllWalls(sizeZ, sizeX);

    }

    private void CreateFloor(int sizeZ, int sizeX){
        Vector3 floorSize = floorPrefab.GetComponent<MeshRenderer>().bounds.size;
        Vector3 wallSize = wallPrefab.GetComponent<MeshRenderer>().bounds.size;
        GameObject newFloor = Instantiate(floorPrefab, transform.position, Quaternion.identity);
        newFloor.transform.localScale = new Vector3(newFloor.transform.localScale.x * sizeX + wallSize.z, 1.0f, newFloor.transform.localScale.z * sizeZ + wallSize.z);
        newFloor.transform.position = new Vector3(newFloor.transform.position.x + ((floorSize.x / 2) * newFloor.transform.localScale.x) - wallSize.z, 
                                                    newFloor.transform.position.y - floorSize.y, 
                                                    newFloor.transform.position.z - ((floorSize.z / 2) * newFloor.transform.localScale.z) + wallSize.z); 
        newFloor.transform.parent = transform;
    }

    // Wall = Broken Wall = 2 * Floor
    // doorWall = 3 * floor
    // door = floor
    private void CreateAllWalls(int sizeZ, int sizeX){
        Vector3 doorSize = doorPrefab.GetComponent<MeshRenderer>().bounds.size;
        Vector3 wallSize = wallPrefab.GetComponent<MeshRenderer>().bounds.size;
        Vector3 wallDoorSize = wallDoorPrefab.GetComponent<MeshRenderer>().bounds.size;
        Vector3 wallCornerSize = wallCornerPrefab.GetComponent<MeshRenderer>().bounds.size;
        
        int[] doorArr = {0, 0, 0, 0}; // North West South East
        int doorSide = Random.Range(0, 4);
        doorArr[doorSide] = 1;
        int doorCord;
        if(doorSide == 0 || doorSide == 1)
            doorCord = Random.Range(1, sizeX/WALL_FACTOR-1);
        else
            doorCord = Random.Range(1, sizeZ/WALL_FACTOR-1);
        int doorPlaced = 0;

        // Left Top
        Vector3 cornerPos = new Vector3(-wallSize.z/2, 0, wallSize.z/2);
        Instantiate(wallCornerPrefab, cornerPos, Quaternion.Euler(new Vector3(0, 90, 0)), transform);
        // Right Top
        cornerPos.x += (sizeX / WALL_FACTOR) * wallSize.x +wallSize.z;
        Instantiate(wallCornerPrefab, cornerPos, Quaternion.Euler(new Vector3(0, 180, 0)), transform);
        // Left Bot
        cornerPos = new Vector3(-wallSize.z/2, 0, - (sizeZ/WALL_FACTOR) * wallSize.x - wallSize.z/2);
        Instantiate(wallCornerPrefab, cornerPos, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
        // Right Bot
        cornerPos.x += (sizeX / WALL_FACTOR) * wallSize.x + wallSize.z;
        Instantiate(wallCornerPrefab, cornerPos, Quaternion.Euler(new Vector3(0, 270, 0)), transform);


        for(int x = 0; x < sizeX / WALL_FACTOR; x++){
            //North
            Vector3 wallPos = new Vector3(wallSize.x/2 + (wallSize.x * x) + ((wallDoorSize.x - wallSize.x) * doorPlaced * doorArr[0]), 
                                            0, 
                                            transform.position.z + wallSize.z/2);
            if(doorArr[0] == 1 && doorCord == x){
                doorPlaced = 1;
                wallPos.x += (wallDoorSize.x - wallSize.x)/2;
                Instantiate(wallDoorPrefab, wallPos, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
                wallPos.x += doorSize.x/2;
                wallPos.y += doorSize.y/10;
                Instantiate(doorPrefab, wallPos, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
                
            }
            else
                Instantiate(wallPrefab, wallPos, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
        
            //South
            wallPos = new Vector3(wallSize.x/2 + (wallSize.x * x) + ((wallDoorSize.x - wallSize.x) * doorPlaced * doorArr[2]), 
                                    0, 
                                    transform.position.z - doorSize.x * sizeZ - wallSize.z/2);
            
            if(doorArr[2] == 1 && doorCord == x){
                doorPlaced = 1;
                wallPos.x += (wallDoorSize.x - wallSize.x)/2;
                Instantiate(wallDoorPrefab, wallPos, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
                wallPos.x += doorSize.x/2;
                wallPos.y += doorSize.y/10;
                Instantiate(doorPrefab, wallPos, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
            }
            else
                Instantiate(wallPrefab, wallPos, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
        }

        for(int z = 0; z < sizeZ / WALL_FACTOR; z++){
            //West
            Vector3 wallPos = new Vector3(transform.position.z - wallSize.z/2, 
                                            0,  
                                            - (wallSize.x/2 + (wallSize.x * z)) - ((wallDoorSize.x - wallSize.x) * doorPlaced * doorArr[1]));
            
            if(doorArr[1] == 1 && doorCord == z){
                doorPlaced = 1;
                wallPos.z -= (wallDoorSize.x - wallSize.x)/2;
                Instantiate(wallDoorPrefab, wallPos, Quaternion.Euler(new Vector3(0, 90, 0)), transform);
                wallPos.z -= doorSize.x/2;
                wallPos.y += doorSize.y/10;
                Instantiate(doorPrefab, wallPos, Quaternion.Euler(new Vector3(0, 90, 0)), transform);
            }
            else
                Instantiate(wallPrefab, wallPos, Quaternion.Euler(new Vector3(0, 90, 0)), transform);

            //East
            wallPos = new Vector3(transform.position.z + doorSize.x * sizeX + wallSize.z/2, 
                                    0,  
                                    - (wallSize.x/2 + (wallSize.x * z)) - ((wallDoorSize.x - wallSize.x) * doorPlaced * doorArr[3]));
            if(doorArr[3] == 1 && doorCord == z){
                doorPlaced = 1;
                wallPos.z -= (wallDoorSize.x - wallSize.x)/2;
                Instantiate(wallDoorPrefab, wallPos, Quaternion.Euler(new Vector3(0, 90, 0)), transform);
                wallPos.z -= doorSize.x/2;
                wallPos.y += doorSize.y/10;
                Instantiate(doorPrefab, wallPos, Quaternion.Euler(new Vector3(0, 90, 0)), transform);
            }
            else
                Instantiate(wallPrefab, wallPos, Quaternion.Euler(new Vector3(0, 90, 0)), transform);
        }
    }
}
