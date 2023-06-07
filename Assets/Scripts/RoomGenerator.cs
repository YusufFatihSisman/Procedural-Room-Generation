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
    public GameObject[] outerObjects;
    public GameObject[] innerObjects;


    private int FLOOR_FACTOR = 6;
    private int WALL_FACTOR = 2;
    private int CELL_FACTOR = 2;
    private int MIN_SIZE = 1;
    private int MAX_SIZE = 3;

    private Vector3 floorSize;
    private Vector3 doorSize;
    private Vector3 wallSize;
    private Vector3 wallDoorSize;
    private Vector3 wallCornerSize;
    Cell[,] cells;

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        floorSize = floorPrefab.GetComponent<MeshRenderer>().bounds.size;
        doorSize = doorPrefab.GetComponent<MeshRenderer>().bounds.size;
        wallSize = wallPrefab.GetComponent<MeshRenderer>().bounds.size;
        wallDoorSize = wallDoorPrefab.GetComponent<MeshRenderer>().bounds.size;
        wallCornerSize = wallCornerPrefab.GetComponent<MeshRenderer>().bounds.size;
        
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
        CreateCells(sizeZ * CELL_FACTOR, sizeX * CELL_FACTOR);

    }

    private void CreateFloor(int sizeZ, int sizeX){
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
        int[] doorArr = {0, 0, 0, 0}; // North West South East
        int doorSide = Random.Range(0, 4);
        doorArr[doorSide] = 1;
        int doorCord;
        if(doorSide == doorArr[0]|| doorSide == doorArr[2])
            doorCord = Random.Range(1, sizeX/WALL_FACTOR - 1);
        else
            doorCord = Random.Range(1, sizeZ/WALL_FACTOR - 1);
        int doorPlaced = 0;

        // Left Top
        Vector3 cornerPos = new Vector3(transform.position.x - wallSize.z/2, transform.position.y , transform.position.z + wallSize.z/2);
        Instantiate(wallCornerPrefab, cornerPos, Quaternion.Euler(new Vector3(0, 90, 0)), transform);
        // Right Top
        cornerPos.x += (sizeX / WALL_FACTOR) * wallSize.x +wallSize.z;
        Instantiate(wallCornerPrefab, cornerPos, Quaternion.Euler(new Vector3(0, 180, 0)), transform);
        // Left Bot
        cornerPos = new Vector3(transform.position.x - wallSize.z/2, transform.position.y, transform.position.z - (sizeZ/WALL_FACTOR) * wallSize.x - wallSize.z/2);
        Instantiate(wallCornerPrefab, cornerPos, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
        // Right Bot
        cornerPos.x += (sizeX / WALL_FACTOR) * wallSize.x + wallSize.z;
        Instantiate(wallCornerPrefab, cornerPos, Quaternion.Euler(new Vector3(0, 270, 0)), transform);


        for(int x = 0; x < sizeX / WALL_FACTOR; x++){
            //North
            Vector3 wallPos = new Vector3(transform.position.x + wallSize.x/2 + (wallSize.x * x) + ((wallDoorSize.x - wallSize.x) * doorPlaced * doorArr[0]), 
                                            transform.position.y , 
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
            wallPos = new Vector3(transform.position.x + wallSize.x/2 + (wallSize.x * x) + ((wallDoorSize.x - wallSize.x) * doorPlaced * doorArr[2]), 
                                    transform.position.y, 
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
            Vector3 wallPos = new Vector3(transform.position.x - wallSize.z/2, 
                                            transform.position.y,  
                                            transform.position.z - (wallSize.x/2 + (wallSize.x * z)) - ((wallDoorSize.x - wallSize.x) * doorPlaced * doorArr[1]));
            
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
            wallPos = new Vector3(transform.position.x + doorSize.x * sizeX + wallSize.z/2, 
                                    transform.position.y,  
                                    transform.position.z - (wallSize.x/2 + (wallSize.x * z)) - ((wallDoorSize.x - wallSize.x) * doorPlaced * doorArr[3]));
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

    private void CreateCells(int sizeZ, int sizeX){
        cells = new Cell[sizeZ, sizeX];

        for(int z = 0; z < sizeZ; z++){
            for(int x = 0; x < sizeX; x++){      
                cells[z, x] = new Cell(new Vector3(transform.position.x + floorSize.x/4 + (x * floorSize.x/2),
                    transform.position.y, 
                    transform.position.z - floorSize.z/4 - (z * floorSize.z/2)));

                if(z == 0)
                    cells[z, x].SetSide(Side.North);
                else if(z == sizeZ - 1)
                    cells[z, x].SetSide(Side.South);
                else if(x == 0)
                    cells[z, x].SetSide(Side.West);
                else if(x == sizeX - 1)
                    cells[z, x].SetSide(Side.East);   

            }
        }

        for(int z = 0; z < sizeZ; z++){
            for(int x = 0; x < sizeX; x++){
                Debug.Log(cells[z, x].position);
            }
        }

        FillCells(sizeZ, sizeX);
    }

    private void FillCells(int sizeZ, int sizeX){
        for(int z = 0; z < sizeZ; z++){
            for(int x = 0; x < sizeX; x++){
                if(cells[z, x].available){
                    int randPlace = 0;
                    randPlace = Random.Range(0, 3);
                    if(randPlace > 0)
                        continue;
                    GameObject choosenObject = ChooseObject(cells[z, x].side);  
                    if(choosenObject == null) // delete when add inside obejct placement
                        continue;

                    // CHECK OBJECT SIZE IF ITS BIGGER THAN THE CeLL SIZE CHANGE POSITION 
                    // SET AVAILABLE STATUS OF NEIGHBOR CELLS
                    Instantiate(choosenObject, cells[z, x].position, Quaternion.Euler(new Vector3(0, 90*(int)cells[z, x].side, 0)), transform); 

                }
            }
        }
    }

    private GameObject ChooseObject(Side side){
        int rand = 0;
        if(side == Side.Inside){
            return null;
            /*
            rand = Random.Range(0, innerObjects.Length);
            return innerObjects[rand];*/
        }
        else{
            rand = Random.Range(0, outerObjects.Length);
            return outerObjects[rand];
        }
            
    }

    public enum Side{
        North,
        West,
        South,
        East,
        Inside,
    }

    public class Cell{
        public Vector3 position;
        public Side side;
        public bool available;

        public Cell(Vector3 pos, Side side = Side.Inside){
            position = pos;
            this.side = side;
            available = true;
        }

        public void SetSide(Side side){
            this.side = side;
        }

        public void SetAvailable(bool newStatus){
            available = newStatus;
        }
    }
}
