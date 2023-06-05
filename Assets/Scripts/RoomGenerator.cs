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
        GameObject newFloor = Instantiate(floorPrefab, transform.position, Quaternion.identity);
        newFloor.transform.localScale = new Vector3(newFloor.transform.localScale.x * sizeX , 1.0f, newFloor.transform.localScale.z * sizeZ);
        newFloor.transform.position = new Vector3(newFloor.transform.position.x + ((floorSize.x / 2) * newFloor.transform.localScale.x), 
                                                    newFloor.transform.position.y, 
                                                    newFloor.transform.position.z - ((floorSize.z / 2) * newFloor.transform.localScale.z)); 
        newFloor.transform.parent = transform;

        /*Vector3 floorSize = floorPrefab.GetComponent<MeshRenderer>().bounds.size;
        for(int z = 0; z < sizeZ; z++){
            for(int x = 0; x < sizeX; x++){
                Vector3 floorPos = new Vector3(floorSize.x/2 + (x * floorSize.x) , 0, floorSize.z/2 + (z * floorSize.z));
                Instantiate(floor, floorPos, Quaternion.identity, transform);
            }
        }
        */
    }

    // Wall = Broken Wall = 2 * Floor
    // doorWall = 3 * floor
    // door = floor
    private void CreateAllWalls(int sizeZ, int sizeX){
        Vector3 doorSize = doorPrefab.GetComponent<MeshRenderer>().bounds.size;
        Vector3 wallSize = wallPrefab.GetComponent<MeshRenderer>().bounds.size;
        Vector3 wallDoorSize = wallDoorPrefab.GetComponent<MeshRenderer>().bounds.size;

        

        int doorSide = Random.Range(0, 4);
        for(int x = 0; x < sizeX / WALL_FACTOR; x++){
            //North
            Vector3 wallPos = new Vector3(wallSize.x/2 + (wallSize.x * x), 0, transform.position.z + wallSize.z/2);
            Instantiate(wallPrefab, wallPos, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
            //South
            wallPos = new Vector3(wallSize.x/2 + (wallSize.x * x), 0, transform.position.z - doorSize.x * sizeZ - wallSize.z/2);
            Instantiate(wallPrefab, wallPos, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
        }

        for(int z = 0; z < sizeZ / WALL_FACTOR; z++){
            //West
            Vector3 wallPos = new Vector3(transform.position.z - wallSize.z/2, 0,  - (wallSize.x/2 + (wallSize.x * z)));
            Instantiate(wallPrefab, wallPos, Quaternion.Euler(new Vector3(0, 90, 0)), transform);
            //East
            wallPos = new Vector3(transform.position.z + doorSize.x * sizeX + wallSize.z/2, 0,  - (wallSize.x/2 + (wallSize.x * z)));
            Instantiate(wallPrefab, wallPos, Quaternion.Euler(new Vector3(0, 90, 0)), transform);
        }
    }

    private void CrateNorthWalls(int sizeZ, int sizeX, Vector3 doorSize, Vector3 wallSize, Vector3 wallDoorSize, bool isDoor){

    }

    private void CrateSouthWalls(int sizeZ, int sizeX, Vector3 doorSize, Vector3 wallSize, Vector3 wallDoorSize, bool isDoor){

    }

    private void CrateEastWalls(int sizeZ, int sizeX, Vector3 doorSize, Vector3 wallSize, Vector3 wallDoorSize, bool isDoor){

    }

    private void CrateWestWalls(int sizeZ, int sizeX, Vector3 doorSize, Vector3 wallSize, Vector3 wallDoorSize, bool isDoor){

    }
}
