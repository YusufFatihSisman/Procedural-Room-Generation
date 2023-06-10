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
    public DecorationAsset[] outerObjects;
    public DecorationAsset[] innerObjects;
    [Range(0,1)]
    public float[] zoneChances;


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
                Instantiate(doorPrefab, wallPos, Quaternion.Euler(new Vector3(0, 180, 0)), transform);
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
                Instantiate(doorPrefab, wallPos, Quaternion.Euler(new Vector3(0, 270, 0)), transform);
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

        FillCells(sizeZ, sizeX);
    }

    private void FillCells(int sizeZ, int sizeX){
        for(int z = 0; z < sizeZ; z++){
            for(int x = 0; x < sizeX; x++){
                if(cells[z, x].available){
                    float randPlace = 0f;
                    randPlace = Random.Range(0f, 1.0f);
                    if(randPlace > zoneChances[(int)cells[z, x].zone])
                        continue;
                    DecorationAsset choosenObject = ChooseObject(cells[z, x].side, cells[z, x].zone);
                    //check object area cells available status  
                    if(choosenObject == null) // delete when add inside obejct placement
                        continue;

                    Vector3 pos = CalculateObjectPosition(sizeZ, sizeX, z, x, cells[z, x].position, cells[z, x].side,choosenObject.area);
                    // CHECK OBJECT SIZE IF ITS BIGGER THAN THE CeLL SIZE CHANGE POSITION 
                    // SET AVAILABLE STATUS OF NEIGHBOR CELLS
                    Instantiate(choosenObject.prefab, pos, Quaternion.Euler(new Vector3(0, 90*(int)cells[z, x].side, 0)), transform); 

                }
            }
        }
    }

   // private void arrangeZones(sizeZ, sizeX, z, x, cells[z, x].position, Vector2 area, Zone zone)

    private Vector3 CalculateObjectPosition(int sizeZ, int sizeX, int z, int x, Vector3 currentPos, Side side, Vector2 area){
        float posx = currentPos.x;
        float posz = currentPos.z;
        if(area.x > 1){
            if(side == Side.East || side == Side.West)
                posz -= floorSize.x/4 * (area.x - floorSize.x/2);
            else
                posx += floorSize.x/4 * (area.x - floorSize.x/2);
        }
        if(area.y > 1){
            if(side == Side.East || side == Side.West)
                posx += floorSize.z/4 * (area.y - floorSize.z/2);
            else
                posz -= floorSize.z/4 * (area.y - floorSize.z/2);
        }

        if(side == Side.East || side == Side.West)
            for(int j = 0; j <= area.y - floorSize.z/2; j++){
                for(int i = 0; i <= area.x - floorSize.x/2; i++){
                    if(x+j < sizeX && z+i < sizeZ)
                        cells[z+i, x+j].SetAvailable(false);
                }
            }
        else
            for(int j = 0; j <= area.y - floorSize.z/2; j++){
                for(int i = 0; i <= area.x - floorSize.x/2; i++){
                    if(x+i < sizeX && z+j < sizeZ)
                        cells[z+j, x+i].SetAvailable(false);
                }
            }
           
        return new Vector3(posx, currentPos.y, posz);    
    }

    private DecorationAsset ChooseObject(Side side, Zone zone){
        int rand = 0;
        if(side == Side.Inside){
            if(zone == Zone.General){
                rand = Random.Range(0, innerObjects.Length);    
                return innerObjects[rand];
            }else{
                List<DecorationAsset> suitables = new List<DecorationAsset>();
                foreach(DecorationAsset i in innerObjects){
                    if(i.zone == zone)
                        suitables.Add(i);
                }
                rand = Random.Range(0, suitables.Count);
                return suitables[rand];  
            }
        }
        else{
            rand = Random.Range(0, outerObjects.Length);
            return outerObjects[rand];
        }
            
    }

    public enum Side{
        South,
        West,
        North,
        East,
        Inside,
    }

    public enum Zone{
        General,
        Table,
        TableAround,
        Pillar
    }

    public class Cell{
        public Vector3 position;
        public Side side;
        public bool available;
        public Zone zone;

        public Cell(Vector3 pos, Side side = Side.Inside){
            position = pos;
            this.side = side;
            available = true;
            zone = Zone.General;
        }

        public void SetSide(Side newSide){
            side = newSide;
        }

        public void SetAvailable(bool newStatus){
            available = newStatus;
        }

        public void SetZone(Zone newZone){
            zone = newZone;
        }
    }
}
