using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public Transform cameraPos;
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
    private Cell[,] cells;

    private int doorSide;
    private int doorCord; 

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
        SetCameraPosition(sizeZ, sizeX);
        CreateFloor(sizeZ, sizeX);
        CreateAllWalls(sizeZ, sizeX);
        CreateCells(sizeZ * CELL_FACTOR, sizeX * CELL_FACTOR);

    }

    private void SetCameraPosition(int sizeZ, int sizeX){
        if(sizeZ <= sizeX)
            cameraPos.position = new Vector3(transform.position.x + sizeX, sizeZ * 3, transform.position.z - sizeZ);
        else{
            cameraPos.position = new Vector3(transform.position.x + sizeX, sizeX * 3, transform.position.z - sizeZ);
            cameraPos.rotation = Quaternion.Euler(new Vector3(cameraPos.rotation.eulerAngles.x, 90, cameraPos.rotation.eulerAngles.z));
        }
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
        int[] doorArr = {0, 0, 0, 0}; // South West North East
        doorSide = Random.Range(0, 4);
        doorArr[doorSide] = 1;
        if(doorArr[0] == 1 || doorArr[2] == 1)
            doorCord = Random.Range(1, (sizeX/WALL_FACTOR) - 1);
        else
            doorCord = Random.Range(1, (sizeZ/WALL_FACTOR) - 1);

        int doorPlaced = 0;
        Debug.Log("SizeX: " + sizeX + "    SizeZ: " + sizeZ);
        Debug.Log("Door Coordinate " + doorCord.ToString());
        Debug.Log("Door Sides " + (Side)doorSide);

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
            Vector3 wallPos = new Vector3(transform.position.x + wallSize.x/2 + (wallSize.x * x) + ((wallDoorSize.x - wallSize.x) * doorPlaced * doorArr[2]), 
                                            transform.position.y , 
                                            transform.position.z + wallSize.z/2);
            if(doorArr[2] == 1 && doorCord == x){
                doorPlaced = 1;
                wallPos.x += (wallDoorSize.x - wallSize.x)/2;
                Instantiate(wallDoorPrefab, wallPos, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
                Instantiate(doorPrefab, wallPos, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
                
            }
            else
                Instantiate(wallPrefab, wallPos, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
        
            //South
            wallPos = new Vector3(transform.position.x + wallSize.x/2 + (wallSize.x * x) + ((wallDoorSize.x - wallSize.x) * doorPlaced * doorArr[0]), 
                                    transform.position.y, 
                                    transform.position.z - doorSize.x * sizeZ - wallSize.z/2);
            
            if(doorArr[0] == 1 && doorCord == x){
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
                Instantiate(doorPrefab, wallPos, Quaternion.Euler(new Vector3(0, 270, 0)), transform);
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

                if(z == 0){
                    if(x == 0 || x == sizeX - 1)
                        cells[z, x].SetAvailable(false);
                    cells[z, x].SetSide(Side.North);
                    cells[z, x].SetEdge(true);    
                }
                else if(z == sizeZ - 1){
                    if(x == 0 || x == sizeX - 1)
                        cells[z, x].SetAvailable(false);
                    cells[z, x].SetSide(Side.South);
                    cells[z, x].SetEdge(true);
                }
                else if(x == 0){
                    cells[z, x].SetSide(Side.West);
                    cells[z, x].SetEdge(true);
                }
                else if(x == sizeX - 1){
                    cells[z, x].SetSide(Side.East);  
                    cells[z, x].SetEdge(true); 
                }

            }
        }
        ClearDoorFront(sizeZ, sizeX);
        FillAllCells(sizeZ, sizeX);
    }

    private void ClearDoorFront(int sizeZ, int sizeX){
        int x = 0;
        int z = 0;
        if(doorSide == (int)Side.South || doorSide == (int)Side.North){
            if(doorSide == (int)Side.South)
                z = sizeZ -1;
            else
                z = 0;
            
            x = WALL_FACTOR * (int)floorSize.x * doorCord - 1;
            for(int i = 0; i < (int)doorSize.x * 2; i++){
                cells[z, x+i+2].SetAvailable(false);
            }
        }else{
            if(doorSide == (int)Side.East)
                x = sizeX -1;
            else
                x = 0;

            z = WALL_FACTOR * (int)floorSize.z * doorCord - 1;
            for(int i = 0; i < (int)doorSize.x * 2; i++){
                cells[z+i+2, x].SetAvailable(false);
            }
        }
    }

    private void FillAllCells(int sizeZ, int sizeX){
        FillEdgeCells(sizeZ, sizeX);
        for(int z = 1; z < sizeZ-1; z++){
            for(int x = 1; x < sizeX-1; x++){
                if(cells[z, x].available && cells[z,x].zone != Zone.Around){
                    FillCell(z, x, sizeZ, sizeX);
                }
            }
        }
    }

    private void FillCell(int z, int x, int sizeZ, int sizeX){
        float randPlace = 0f;
        randPlace = Random.Range(0f, 1.0f);
        if(randPlace > zoneChances[(int)cells[z, x].zone])
            return;
        DecorationAsset choosenObject = ChooseObject(cells[z, x].zone, cells[z, x].edge);
        if(choosenObject == null)
            return;
        if(!IsObjectAreaFit(sizeZ, sizeX, z, x, cells[z, x].side, choosenObject.area, choosenObject.zone))
            return;
        Vector3 pos = CalculateObjectPosition(sizeZ, sizeX, z, x, cells[z, x].position, cells[z, x].side, choosenObject.area, choosenObject.aroundZone);
        if(choosenObject.prefab.name == "TorchWall")
            pos = setTorch(pos, cells[z, x].side, cells[z,x].edge); 
        if(cells[z, x].edge)
            ClearEdgeFront(z, x, cells[z, x].side, choosenObject.area);
        if(cells[z, x].zone == Zone.Pillar)
            Instantiate(choosenObject.prefab, pos, Quaternion.Euler(new Vector3(0, 90*(((int)cells[z, x].side + 2) % 4), 0)), transform);
        else
            Instantiate(choosenObject.prefab, pos, Quaternion.Euler(new Vector3(0, 90*(int)cells[z, x].side, 0)), transform); 
    }

    private void FillEdgeCells(int sizeZ, int sizeX){
        int z = 0;
        int x = 0;
        while(z < sizeZ){
            for(x = 0; x < sizeX; x++){
                FillCell(z, x, sizeZ, sizeX);           
            }
            z += sizeZ - 1;
        }       
        x = 0;
        while(x < sizeX){
            for(z = 0; z < sizeZ; z++){
                FillCell(z, x, sizeZ, sizeX);
            }
            x += sizeX - 1;
        }
    }

    private Vector3 setTorch(Vector3 pos, Side side, bool edge){
        pos.y = wallSize.y/2;
        if(edge){
            if(side == Side.East)
                pos.x += floorSize.x/4;
            if(side == Side.West)
                pos.x -= floorSize.x/4;   
            if(side == Side.South)
                pos.z -= floorSize.z/4;
            if(side == Side.North)
                pos.z += floorSize.z/4;
        }else{
            if(side == Side.East)
                pos.x -= floorSize.x/2;
            if(side == Side.West)
                pos.x += floorSize.x/2;   
            if(side == Side.South)
                pos.z += floorSize.z/2;
            if(side == Side.North)
                pos.z -= floorSize.z/2;
        }
        return pos;
    }

    private void ClearEdgeFront(int z, int x, Side side, Vector2 area){    
        if(side == Side.South)
            for(int i = 0; i <= area.x - floorSize.x/2; i++)
                cells[z-1, x+i].SetAvailable(false);
        else if(side == Side.North)
            for(int i = 0; i <= area.x - floorSize.x/2; i++)
                cells[z+1, x+i].SetAvailable(false);
        else if(side == Side.East)
            for(int i = 0; i <= area.x - floorSize.z/2; i++)
                cells[z+i, x-1].SetAvailable(false);
        else if(side == Side.West)
            for(int i = 0; i <= area.x - floorSize.z/2; i++)
                    cells[z+i, x+1].SetAvailable(false);
    }

    private bool IsObjectAreaFit(int sizeZ, int sizeX, int z, int x, Side side, Vector2 area, Zone zone){     
        if(side == Side.East || side == Side.West)
            for(int j = 0; j <= area.y - floorSize.z/2; j++){
                for(int i = 0; i <= area.x - floorSize.x/2; i++){
                    if(x+j < sizeX && z+i < sizeZ){
                        if(cells[z+i, x+j].available == false)
                            return false;
                        if(zone == Zone.General && cells[z+i, x+j].zone == Zone.Around)
                            return false;        
                    }       
                    else
                        return false;
                }
            }
        else
            for(int j = 0; j <= area.y - floorSize.z/2; j++){
                for(int i = 0; i <= area.x - floorSize.x/2; i++){
                    if(x+i < sizeX && z+j < sizeZ){
                        if(cells[z+j, x+i].available == false)
                            return false;
                        if(zone == Zone.General && cells[z+j, x+i].zone == Zone.Around)
                            return false;
                    }
                    else
                        return false;
                }
            }
        return true;   
    }

    private void SetNeighboorZones(int sizeZ, int sizeX, int zTop, int zBot, int xLeft, int xRight, Zone aroundZone){
        if(aroundZone == Zone.General)
            return;
        for(int z = zTop - 2; z <= zBot + 2; z++){
            for(int x = xLeft - 2; x <= xRight + 2; x++){
                if(z < sizeZ && z >= 0 && x < sizeX && x >= 0){
                    if(((x >= xLeft && x <= xRight) && (z == zTop - 2 || z == zBot + 2))
                        || (z >= zTop && z <= zBot) && (x == xLeft - 2 || x == xRight + 2)){
                        if(aroundZone == Zone.Pillar)
                            cells[z,x].SetAvailable(false);
                        else
                            cells[z,x].SetZone(Zone.Around);
                    }
                    else if((z == zBot + 1 || z == zTop - 1) && (x == xLeft-1 || x == xRight + 1))
                        cells[z,x].SetAvailable(false);
                    else if(((z == zTop - 1 || z == zBot + 1) && (x >= xLeft && x <= xRight)) 
                        || (x == xLeft - 1 || x == xRight + 1) && (z >= zTop && z <= zBot)){
                            if(z > zBot)
                                if(z != sizeZ - 1)
                                    cells[z, x].SetSide(Side.South);
                            if(x > xRight)
                                if(x != sizeX - 1)
                                    cells[z, x].SetSide(Side.East);
                            if(x < xLeft)
                                if(z != sizeX - 1)
                                    cells[z, x].SetSide(Side.West);
                            cells[z,x].SetZone(aroundZone);
                            if(cells[z,x].available)
                                FillCell(z, x, sizeZ, sizeX);
                        }
                }
            }
        }
    }

    private Vector3 CalculateObjectPosition(int sizeZ, int sizeX, int z, int x, Vector3 currentPos, Side side, Vector2 area, Zone aroundZone){
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
        int j = 0;
        int i = 0;
        if(side == Side.East || side == Side.West){
            for(j = 0; j <= area.y - floorSize.z/2; j++){
                for(i = 0; i <= area.x - floorSize.x/2; i++){
                    if(x+j < sizeX && z+i < sizeZ)
                        cells[z+i, x+j].SetAvailable(false);
                }
            }
            SetNeighboorZones(sizeZ, sizeX, z, z+i-1, x, x+j-1, aroundZone);
        }
        else{
            for(j = 0; j <= area.y - floorSize.z/2; j++){
                for(i = 0; i <= area.x - floorSize.x/2; i++){
                    if(x+i < sizeX && z+j < sizeZ)
                        cells[z+j, x+i].SetAvailable(false);
                }
            }
            SetNeighboorZones(sizeZ, sizeX, z, z+j-1, x, x+i-1, aroundZone);
        }   
        return new Vector3(posx, currentPos.y, posz);    
    }

    private DecorationAsset ChooseObject(Zone zone, bool edge){
        int rand = 0;
        if(!edge){
            List<DecorationAsset> suitables = new List<DecorationAsset>();
            foreach(DecorationAsset i in innerObjects){
                if(i.zone == zone){
                    suitables.Add(i);
                    if(i.prefab.name == "TorchWall")
                        suitables.Add(i);
                }
            }
            if(suitables.Count == 0)
                return null;
            rand = Random.Range(0, suitables.Count);
            return suitables[rand];  
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
    }

    public enum Zone{
        General,
        Table,
        Pillar,
        Around,
    }

    public class Cell{
        public Vector3 position;
        public Side side;
        public bool available;
        public Zone zone;
        public bool edge;

        public Cell(Vector3 pos, Side side = Side.North, bool edge = false){
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

        public void SetEdge(bool edgeStatus){
            edge = edgeStatus;
        }
    }
}
