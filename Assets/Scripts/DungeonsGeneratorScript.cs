using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BuildStruct
{
    public Ivec3 from, to;
    public int remainingLife, id, fromId, currentExpand;
    public bool isMain;
}

[System.Serializable]
public struct CorridorBuildStruct
{
    public Ivec3 toPos, fromPos, middlePos;
}

public enum CorridorType
{
    connectAllPossible,
    connectOnlyBuildOrder,
    tangledTree
}

public class DungeonsGeneratorScript : MonoBehaviour
{
    public enum DeffineStartRoomPos
    {
        middle,
        customOffsets
    }

    public Camera camera;
    List<CorridorBuildStruct> corridorsLeftToBuild = new List<CorridorBuildStruct>();
    [SerializeField] GameObject player;
    [SerializeField] GameObject floorTile, wallTile;
    [SerializeField] bool generateWalls;
    [Range(4, 100)] public int dungeonHeight, dungeonWidh;
    [SerializeField] [Range(0, 100)] float cellSizePrecentUsage = 75;
    [SerializeField] int maxSideExpandLength = 1;


    [Tooltip("how many floor neighbors a wall tile need to have to change to a floor tile")]
    [SerializeField] [Range(0, 8)] int cleanUpReqTiles;
    [Tooltip("amount of times the the search will iterate, notcie that a iteration will not effect those floor tiles that changed in the same iteration")]
    [SerializeField] int cleanUpLoops;

    [HideInInspector] public DeffineStartRoomPos StartRoomPos;
    [HideInInspector] public int startRoomBuildWidthOffset, startRoomBuildHeightOffset;


   [HideInInspector] [SerializeField] public GameObject oldHolder;
   [SerializeField] public int[,] worldMap;
   [SerializeField] int[] serializedMapArray;
    public int amountOfRooms;

    [SerializeField] public CorridorType corridorType = CorridorType.connectAllPossible;
    [HideInInspector] public float precantageOfBuild;

    [SerializeField] int corridorMinOffsetWidth, corridorMaxOffsetWidth, corridorMinOffsetHeight, corridorMaxOffsetHeight;
    int currentAtempt = 0;


    [SerializeField] bool drawGizmozDirection;
    [SerializeField] bool drawGizmozMain;
    [SerializeField] bool drawGizmozCorridors;
    [SerializeField] bool drawGizmozDisplayRoom;
    [SerializeField] int roomDisplayIndex;
    [SerializeField] DungeonElementPlacement dungeonElementPlacement;
    private void OnValidate()
    {
        if (roomDisplayIndex <= 0)
        {
            roomDisplayIndex = 0;
        }
        if (roomDisplayIndex >= closedPosition.Count)
        {
            roomDisplayIndex = closedPosition.Count - 1;
        }
        if (cleanUpLoops < 0)
        {
            cleanUpLoops = 0;
        }
        if (corridorMaxOffsetHeight >= (dungeonHeight / 2) - 1)
        {
            corridorMaxOffsetHeight = (dungeonHeight / 2) - 1;
        }
        if (corridorMaxOffsetWidth >= (dungeonWidh / 2) - 1)
        {
            corridorMaxOffsetWidth = (dungeonWidh / 2) - 1;
        }
        if (corridorMaxOffsetHeight < corridorMinOffsetHeight)
        {
            corridorMaxOffsetHeight = corridorMinOffsetHeight;
        }
        if (corridorMaxOffsetWidth < corridorMinOffsetWidth)
        {
            corridorMaxOffsetWidth = corridorMinOffsetWidth;
        }
        if (corridorMinOffsetHeight < 0)
        {
            corridorMinOffsetHeight = 0;
        }
        if (corridorMinOffsetWidth < 0)
        {
            corridorMinOffsetWidth = 0;
        }
    }
    private void Awake()
    {
        /*for(int i = 0; i < serializedMapArray.Length; i++)
        {
            int x = dungeonWidh % i;
            worldMap[x, z] = serializedMapArray[i];
        }*/
        temp();
    }
    void temp()
    {
        worldMap = new int[
           (dungeonWidh * amountOfRooms),
           (dungeonHeight * amountOfRooms)];
        int i = 0;
        for (int x = 0; x < dungeonWidh * amountOfRooms; x++)
        {
            for (int z = 0; z < dungeonHeight * amountOfRooms; z++)
            {
                worldMap[x, z] = serializedMapArray[i];
                i++;
            }
        }
    }
    void setWorldArray()
    {
        serializedMapArray = new int[dungeonWidh * dungeonHeight * amountOfRooms * amountOfRooms];
        int i = 0;
        for (int x = 0; x < dungeonWidh * amountOfRooms; x++)
        {
            for (int z = 0; z < dungeonHeight * amountOfRooms; z++)
            {
                serializedMapArray[i] = 
                    worldMap[x, z];
                i++;
            }
        }
    }
    List<BuildStruct> removePosition(List<BuildStruct> currentList, Ivec3 from)
    {
        List<BuildStruct> returnValue = currentList;
        int x = returnValue.Count;
        for (int i = currentList.Count - 1; i >= 0; i--)
        {
            if (currentList[i].from.Equals(from))
            {
                currentList.RemoveAt(i);
            }
        }
        //Debug.Log(returnValue.Count + ":" + x);
        return returnValue;
    }
    bool isValidBS(BuildStruct BS, List<BuildStruct> possiblePosition, List<BuildStruct> closedPosition)
    {
        if (!(BS.from.x >= 0 && BS.from.z >= 0))
        {
            return false;
        }
        if (!(BS.to.x <= dungeonWidh * amountOfRooms &&
            BS.to.z <= dungeonHeight * amountOfRooms))
        {
            return false;
        }
        //this should not be here anymore
        /* for (int i = 0; i < possiblePosition.Count; i++)
         {
             if (possiblePosition[i].from.x == BS.from.x &&
                 possiblePosition[i].from.z == BS.from.z)
             {
                 return false;
             }
         }*/
        for (int i = 0; i < closedPosition.Count; i++)
        {
            if (closedPosition[i].from.x == BS.from.x &&
                closedPosition[i].from.z == BS.from.z)
            {
                return false;
            }
        }
        return true;
    }

    int[,] updateCorrList(int[,] currentList, Ivec3 from, Ivec3 to)
    {
        int[,] tempArr = currentList;

        //här kan optimeras
        int x1 = from.x;
        int y1 = from.z;
        int x2 = to.x;
        int y2 = to.z;

        Vector2 current = new Vector2(x1, y1);
        Vector2 direction = new Vector2(x2, y2) - current;
        int jumpsNeeded = (int)direction.magnitude;
        direction.Normalize();

        for (int i = 0; i <= jumpsNeeded; i++)
        {
            tempArr[(int)current.x, (int)current.y] = 2;
            worldMap[(int)current.x, (int)current.y] = 0;

            current += direction;
        }
        return tempArr;
    }

     List<BuildStruct> possiblePosition = new List<BuildStruct>();
    [SerializeField] [HideInInspector] public List<BuildStruct> closedPosition = new List<BuildStruct>();
    public void GenerateDungeon(bool firstGen = true, bool inGame = false)
    {
        if (firstGen)
        {
            currentAtempt = 0;
        }
        possiblePosition.Clear();
        closedPosition.Clear();
        worldMap = new int[
           (dungeonWidh * amountOfRooms),
           (dungeonHeight * amountOfRooms)];


        if (oldHolder != null)
        {
            if (inGame)
            {
                Destroy(oldHolder);
            }
            else
            {                
                DestroyImmediate(oldHolder);
            }
            
        }
        else
        {
            oldHolder = GameObject.Find("parent");
            if (oldHolder != null)
            {
                DestroyImmediate(oldHolder);
            }
        }
        corridorsLeftToBuild.Clear();
        GameObject dungeon = new GameObject("parent");
        oldHolder = dungeon;

        //list goes here

        BuildStruct BS = new BuildStruct();
        BS.from = new Ivec3(startRoomBuildWidthOffset * dungeonWidh, 0, startRoomBuildHeightOffset * dungeonHeight);  //old system, might change back
        //BS.from = new Ivec3(0,0,(int)(dungeonHeight*amountOfRooms/2));
        BS.to = BS.from + new Ivec3(dungeonWidh, 0, dungeonHeight);//asum
        ///BS.id = 0;
        BS.isMain = true;
        BS.currentExpand = 0;
        possiblePosition.Add(BS);

        for (int counter = 0; counter < amountOfRooms; counter++)
        {
            for (int i = possiblePosition.Count - 1; i >= 0; i--)
            {
                possiblePosition[i].remainingLife--;
                if (/*possiblePosition[i].remainingLife < 0 ||*/ possiblePosition[i].currentExpand > maxSideExpandLength)
                {
                    possiblePosition.RemoveAt(i);
                }
            }

            if (possiblePosition.Count <= 0 && currentAtempt <= 5)
            {
                if (currentAtempt >= 5)
                {
                    Debug.LogError("unexpected error, dungen surrounded the current path");
                }
                GenerateDungeon(false);
                return;
            }
            int randomPosIndex = Random.Range(0, possiblePosition.Count - 1);
            int thisRoomId = counter;// + 1;
            possiblePosition[randomPosIndex].id = thisRoomId;


            if (possiblePosition[randomPosIndex].isMain)
            {
                foreach (BuildStruct mainBS in possiblePosition)
                {
                    mainBS.isMain = false;
                }
                possiblePosition[randomPosIndex].isMain = true;
            }
            closedPosition.Add(possiblePosition[randomPosIndex]);
            
            bool isMain = possiblePosition[randomPosIndex].isMain;
            int nextExpand = isMain ? 0 : (possiblePosition[randomPosIndex].currentExpand + 1);

            Ivec3 currentFrom = possiblePosition[randomPosIndex].from;
            Ivec3 currentTo = possiblePosition[randomPosIndex].to;

            if (nextExpand < maxSideExpandLength)
            {
                BuildStruct BSright = new BuildStruct();
                BSright.isMain = isMain;
                BSright.from = currentFrom + new Ivec3(dungeonWidh, 0, 0);
                BSright.to = currentTo + new Ivec3(dungeonWidh, 0, 0);
                BSright.fromId = thisRoomId;
                BSright.currentExpand = nextExpand;
                if (isValidBS(BSright, possiblePosition, closedPosition))
                {
                    possiblePosition.Add(BSright);
                }


                BuildStruct BSleft = new BuildStruct();
                BSleft.isMain = isMain;
                BSleft.from = currentFrom + new Ivec3(-dungeonWidh, 0, 0);
                BSleft.to = currentTo + new Ivec3(-dungeonWidh, 0, 0);
                BSleft.fromId = thisRoomId;
                BSleft.currentExpand = nextExpand;
                if (isValidBS(BSleft, possiblePosition, closedPosition))
                {
                    possiblePosition.Add(BSleft);
                }


                BuildStruct BSup = new BuildStruct();
                BSup.isMain = isMain;
                BSup.from = currentFrom + new Ivec3(0, 0, dungeonHeight);
                BSup.to = currentTo + new Ivec3(0, 0, dungeonHeight);
                BSup.fromId = thisRoomId;
                BSup.currentExpand = nextExpand;
                if (isValidBS(BSup, possiblePosition, closedPosition))
                {
                    possiblePosition.Add(BSup);
                }

                BuildStruct BSdown = new BuildStruct();
                BSdown.isMain = isMain;
                BSdown.from = currentFrom + new Ivec3(0, 0, -dungeonHeight);
                BSdown.to = currentTo + new Ivec3(0, 0, -dungeonHeight);
                //BSdown.remainingLife = inheritedLifeTime ? (possiblePosition[randomPosIndex].isMain ? cellPosLifeTime + 1 : closedPosition[possiblePosition[randomPosIndex].fromId].remainingLife - 1) : cellPosLifeTime + 1;
                BSdown.fromId = thisRoomId;
                BSdown.currentExpand = nextExpand;
                if (isValidBS(BSdown, possiblePosition, closedPosition))
                {
                    possiblePosition.Add(BSdown);
                }
            }




            int[,] arr = new int[dungeonWidh, dungeonHeight];

            if (dungeonWidh < 4 || dungeonHeight < 4)
            {
                Debug.LogError("cell to small");
                return;
            }
            int widthStart = (int)(dungeonWidh / 2);//Random.Range((int)(dungeonWidh / 2) - 1, (int)(dungeonWidh / 2) + 1);
            int heightStart = (int)(dungeonHeight / 2); //Random.Range((int)(dungeonHeight / 2) - 1, (int)(dungeonHeight / 2) + 1);

            if (counter == 0)
            {
                //set playerStartposition
                Vector3 startPos = new Vector3(widthStart, 0, heightStart) +
                    new Vector3(widthStart, 0, heightStart - 1) +
                    new Vector3(widthStart - 1, 0, heightStart - 1) +
                    new Vector3(widthStart - 1, 0, heightStart);
                startPos = startPos / 4;
                startPos += new Vector3(startRoomBuildWidthOffset * dungeonWidh, 0, startRoomBuildHeightOffset * dungeonHeight);
                startPos += new Vector3(0, 0.75f, 0);
                player.transform.position = startPos;
            }

            //set small startRoom to build upon 
            arr[widthStart, heightStart] = 2;
            arr[widthStart, heightStart - 1] = 2;
            arr[widthStart - 1, heightStart - 1] = 2;
            arr[widthStart - 1, heightStart] = 2;

            int totalFloorRooms = 4;
            int totaDesiredFloorTiles = (int)(((dungeonWidh * dungeonHeight) - ((dungeonWidh * 2 + dungeonHeight * 2)) + 4) * (cellSizePrecentUsage * 0.01f));

            List<Ivec3> possibleExpandings = new List<Ivec3>();
            possibleExpandings.Add(new Ivec3(widthStart, 0, heightStart - 2));
            possibleExpandings.Add(new Ivec3(widthStart - 1, 0, heightStart - 2));
            possibleExpandings.Add(new Ivec3(widthStart + 1, 0, heightStart - 1));
            possibleExpandings.Add(new Ivec3(widthStart + 1, 0, heightStart));
            possibleExpandings.Add(new Ivec3(widthStart - 2, 0, heightStart - 1));
            possibleExpandings.Add(new Ivec3(widthStart - 2, 0, heightStart));
            possibleExpandings.Add(new Ivec3(widthStart, 0, heightStart + 1));
            possibleExpandings.Add(new Ivec3(widthStart - 1, 0, heightStart + 1));

            List<Ivec3> closedExpandings = new List<Ivec3>();
            closedExpandings.Add(new Ivec3(widthStart, 0, heightStart));
            closedExpandings.Add(new Ivec3(widthStart, 0, heightStart - 1));
            closedExpandings.Add(new Ivec3(widthStart - 1, 0, heightStart - 1));
            closedExpandings.Add(new Ivec3(widthStart - 1, 0, heightStart));

            //get the amount of floor tiles that should exist
            while (totalFloorRooms <= totaDesiredFloorTiles)
            {
                int expandIndex = Random.Range(0, possibleExpandings.Count);
                arr[possibleExpandings[expandIndex].x, possibleExpandings[expandIndex].z] = 2;
                totalFloorRooms++;
                closedExpandings.Add(possibleExpandings[expandIndex]);
                Ivec3 currentPosExpand = possibleExpandings[expandIndex];
                possibleExpandings.RemoveAt(expandIndex);


                //if ((currentPosExpand.x >= 2) && (currentPosExpand.x < dungeonWidh - 2) && (currentPosExpand.z >= 2) && (currentPosExpand.z < dungeonHeight - 2))
                {
                    Ivec3 up = currentPosExpand + new Ivec3(0, 0, 1);
                    if (currentPosExpand.z < dungeonHeight - 2 && !Ivec3.Contains(closedExpandings, up) && !Ivec3.Contains(possibleExpandings, up))
                    {
                        possibleExpandings.Add(up);
                    }
                    Ivec3 down = currentPosExpand + new Ivec3(0, 0, -1);
                    if (currentPosExpand.z >= 2 && !Ivec3.Contains(closedExpandings, down) && !Ivec3.Contains(possibleExpandings, down))
                    {
                        possibleExpandings.Add(down);
                    }
                    Ivec3 right = currentPosExpand + new Ivec3(1, 0, 0);
                    if (currentPosExpand.x < dungeonWidh - 2 && !Ivec3.Contains(closedExpandings, right) && !Ivec3.Contains(possibleExpandings, right))
                    {
                        possibleExpandings.Add(right);
                    }
                    Ivec3 left = currentPosExpand + new Ivec3(-1, 0, 0);
                    if (currentPosExpand.x >= 2 && !Ivec3.Contains(closedExpandings, left) && !Ivec3.Contains(possibleExpandings, left))
                    {
                        possibleExpandings.Add(left);
                    }
                }
            }
            for (int i = 0; i < cleanUpLoops; i++)
            {
                int[,] thisIterationArr = arr;

                for (int widthNr = 0; widthNr < dungeonWidh; widthNr++)
                {
                    for (int heightNr = 0; heightNr < dungeonHeight; heightNr++)
                    {
                        if ((widthNr >= 1) && (widthNr < dungeonWidh - 1) && (heightNr >= 1) && (heightNr < dungeonHeight - 1))
                        {
                            int amountOfValidNeighbors = 0;
                            #region look at neighbours 
                            //u
                            if (thisIterationArr[widthNr, heightNr + 1] == 2)
                            {
                                amountOfValidNeighbors++;
                            }
                            //ur
                            if (thisIterationArr[widthNr + 1, heightNr + 1] == 2)
                            {
                                amountOfValidNeighbors++;
                            }
                            //r
                            if (thisIterationArr[widthNr + 1, heightNr] == 2)
                            {
                                amountOfValidNeighbors++;
                            }
                            //dr
                            if (thisIterationArr[widthNr + 1, heightNr - 1] == 2)
                            {
                                amountOfValidNeighbors++;
                            }
                            //d
                            if (thisIterationArr[widthNr, heightNr - 1] == 2)
                            {
                                amountOfValidNeighbors++;
                            }
                            //dl
                            if (thisIterationArr[widthNr - 1, heightNr - 1] == 2)
                            {
                                amountOfValidNeighbors++;
                            }
                            //l
                            if (thisIterationArr[widthNr - 1, heightNr] == 2)
                            {
                                amountOfValidNeighbors++;
                            }
                            //ul
                            if (thisIterationArr[widthNr - 1, heightNr + 1] == 2)
                            {
                                amountOfValidNeighbors++;
                            }
                            #endregion

                            if (amountOfValidNeighbors >= cleanUpReqTiles)
                            {
                                arr[widthNr, heightNr] = 2;
                            }
                        }

                    }
                }
            }

            //add arr to the world map
            for (int widthNr = 0; widthNr < dungeonWidh; widthNr++)
            {
                for (int heightNr = 0; heightNr < dungeonHeight; heightNr++)
                {
                    worldMap[currentFrom.x + widthNr, currentFrom.z + heightNr] = arr[widthNr, heightNr];
                }
            }
            possiblePosition = removePosition(possiblePosition, possiblePosition[randomPosIndex].from);
            //possiblePosition.RemoveAt(randomPosIndex);
        }


        Ivec3 corridorOffset = new Ivec3((int)(dungeonWidh / 2), 0, (int)(dungeonHeight / 2));
        //clear corridor if certain type
        if (corridorType == CorridorType.connectOnlyBuildOrder || corridorType == CorridorType.tangledTree)
        {
            for (int i = 1; i < closedPosition.Count; i++)
            {
                CorridorBuildStruct newCorridor = new CorridorBuildStruct();
                newCorridor.fromPos = closedPosition[closedPosition[i].fromId].from + corridorOffset;
                newCorridor.toPos = closedPosition[i].from + corridorOffset;
                newCorridor.middlePos = (newCorridor.fromPos + newCorridor.toPos) / 2 +
                           new Ivec3(
                               Random.Range(corridorMinOffsetWidth, corridorMaxOffsetWidth) * (Random.Range(0, 2) == 1 ? -1 : 1),
                               0,
                               Random.Range(corridorMinOffsetHeight, corridorMaxOffsetHeight) * (Random.Range(0, 2) == 1 ? -1 : 1));

                corridorsLeftToBuild.Add(newCorridor);
            }
        }

        //buildCorridor
        //List<CorridorBuildStruct> corridorsLeftToBuild = new List<CorridorBuildStruct>();

        //deffine all corridors if type of corridor is to connect all possible rooms
        if (corridorType == CorridorType.connectAllPossible)
        {
            for (int i = 0; i < closedPosition.Count; i++)
            {
                for (int j = i + 1; j < closedPosition.Count; j++)
                {
                    //distance formula to calculate well the distance in x and z axel to next room center
                    float distanceX = Mathf.Sqrt(Mathf.Pow((closedPosition[i].from.x - closedPosition[j].from.x), 2));
                    float distanceZ = Mathf.Sqrt(Mathf.Pow((closedPosition[i].from.z - closedPosition[j].from.z), 2));

                    //if distance match and is on different levels (so that diagonally don't count, create a corridor)
                    if ((distanceX <= dungeonWidh && distanceZ <= dungeonHeight) &&
                        ((closedPosition[i].from.x != closedPosition[j].from.x && closedPosition[i].from.z == closedPosition[j].from.z) ||
                        (closedPosition[i].from.x == closedPosition[j].from.x && closedPosition[i].from.z != closedPosition[j].from.z)))
                    {
                        CorridorBuildStruct CBS = new CorridorBuildStruct();
                        CBS.fromPos = new Ivec3(closedPosition[i].from.x, 0, closedPosition[i].from.z) + new Ivec3((int)(dungeonWidh / 2), 0, (int)(dungeonHeight / 2));
                        CBS.toPos = new Ivec3(closedPosition[j].from.x, 0, closedPosition[j].from.z) + new Ivec3((int)(dungeonWidh / 2), 0, (int)(dungeonHeight / 2));

                        CBS.middlePos = (CBS.fromPos + CBS.toPos) / 2 +
                            new Ivec3(
                                Random.Range(corridorMinOffsetWidth, corridorMaxOffsetWidth) * (Random.Range(0, 2) == 1 ? -1 : 1),
                                0,
                                Random.Range(corridorMinOffsetHeight, corridorMaxOffsetHeight) * (Random.Range(0, 2) == 1 ? -1 : 1));

                        corridorsLeftToBuild.Add(CBS);
                    }
                }
            }
        }
        else if (corridorType == CorridorType.tangledTree)
        {
            for (int i = 0; i < closedPosition.Count; i++)
            {
                for (int j = i + 1; j < closedPosition.Count; j++)
                {
                    float randomValue = Random.Range(0, 100);
                    if(randomValue >= precantageOfBuild)
                    {
                        continue;
                    }

                    Ivec3 fromPos = new Ivec3(closedPosition[i].from.x, 0, closedPosition[i].from.z) + new Ivec3((int)(dungeonWidh / 2), 0, (int)(dungeonHeight / 2));
                    Ivec3 toPos = new Ivec3(closedPosition[j].from.x, 0, closedPosition[j].from.z) + new Ivec3((int)(dungeonWidh / 2), 0, (int)(dungeonHeight / 2));

                    bool found = false;
                    for(int k = 0; k < corridorsLeftToBuild.Count; k++)
                    {
                        if((corridorsLeftToBuild[k].fromPos.Equals(fromPos) && corridorsLeftToBuild[k].toPos.Equals(toPos)) ||
                            (corridorsLeftToBuild[k].fromPos.Equals(toPos) && corridorsLeftToBuild[k].toPos.Equals(fromPos)))
                        {
                            found = true;
                            break;
                        }
                    }
                    if(found)
                    {
                        continue;
                    }
                    //distance formula to calculate well the distance in x and z axel to next room center
                    float distanceX = Mathf.Sqrt(Mathf.Pow((closedPosition[i].from.x - closedPosition[j].from.x), 2));
                    float distanceZ = Mathf.Sqrt(Mathf.Pow((closedPosition[i].from.z - closedPosition[j].from.z), 2));

                    //if distance match and is on different levels (so that diagonally don't count, create a corridor)
                    if ((distanceX <= dungeonWidh && distanceZ <= dungeonHeight) &&
                        ((closedPosition[i].from.x != closedPosition[j].from.x && closedPosition[i].from.z == closedPosition[j].from.z) ||
                        (closedPosition[i].from.x == closedPosition[j].from.x && closedPosition[i].from.z != closedPosition[j].from.z)))
                    {
                        CorridorBuildStruct CBS = new CorridorBuildStruct();
                        CBS.fromPos = new Ivec3(closedPosition[i].from.x, 0, closedPosition[i].from.z) + new Ivec3((int)(dungeonWidh / 2), 0, (int)(dungeonHeight / 2));
                        CBS.toPos = new Ivec3(closedPosition[j].from.x, 0, closedPosition[j].from.z) + new Ivec3((int)(dungeonWidh / 2), 0, (int)(dungeonHeight / 2));

                        CBS.middlePos = (CBS.fromPos + CBS.toPos) / 2 +
                            (Ivec3) new Vector3(
                                Random.Range(corridorMinOffsetWidth, corridorMaxOffsetWidth) * (Random.Range(0, 2) == 1 ? -1 : 1),
                                0,
                                Random.Range(corridorMinOffsetHeight, corridorMaxOffsetHeight) * (Random.Range(0, 2) == 1 ? -1 : 1));

                        //wall fix (sometimes didnt create wills since corridor was outside of limits)
                        if (CBS.middlePos.x < 0)
                        {
                            CBS.middlePos.x++;
                        }
                        else if(CBS.middlePos.x > dungeonWidh * amountOfRooms)
                        {
                            CBS.middlePos.x--;
                        }
                        if (CBS.middlePos.y < 0)
                        {
                            CBS.middlePos.y++;
                        }
                        else if (CBS.middlePos.y > dungeonHeight * amountOfRooms)
                        {
                            CBS.middlePos.y--;
                        }

                        corridorsLeftToBuild.Add(CBS);
                    }
                }
            }
        }
        int[,] corrArr = new int[
            (dungeonWidh * amountOfRooms),
           (dungeonHeight * amountOfRooms)];

        for (int i = 0; i < corridorsLeftToBuild.Count; i++)
        {
            corrArr = updateCorrList(corrArr, corridorsLeftToBuild[i].middlePos, corridorsLeftToBuild[i].fromPos);
            corrArr = updateCorrList(corrArr, corridorsLeftToBuild[i].middlePos, corridorsLeftToBuild[i].toPos);
        }

        int[,] corrArrExpand = (int[,])corrArr.Clone();
        for (int widthNr = 1; widthNr < dungeonWidh * amountOfRooms - 1; widthNr++)
        {
            for (int heightNr = 1; heightNr < dungeonHeight * amountOfRooms -1; heightNr++)
            {
                if (worldMap[widthNr, heightNr] != 0) continue;


                if (corrArrExpand[widthNr, heightNr] == 0)
                {
                        if (corrArrExpand[widthNr + 1, heightNr] == 2)
                        {
                            corrArr[widthNr, heightNr] = 2;
                            continue;
                        }
                    
                        if (corrArrExpand[widthNr - 1, heightNr] == 2)
                        {
                            corrArr[widthNr, heightNr] = 2;
                            continue;
                        }
                    
                        if (corrArrExpand[widthNr, heightNr + 1] == 2)
                        {
                            corrArr[widthNr, heightNr] = 2;
                            continue;
                        }
                    
                        if (corrArrExpand[widthNr, heightNr - 1] == 2)
                        {
                            corrArr[widthNr, heightNr] = 2;
                            continue;
                        }
                    
                }
            }
        }
        //add corridors to the global map
        for (int widthNr = 1; widthNr < dungeonWidh * amountOfRooms - 1; widthNr++)
        {
            for (int heightNr = 1; heightNr < dungeonHeight * amountOfRooms - 1; heightNr++)
            {
                worldMap[widthNr, heightNr] = (worldMap[widthNr, heightNr] == 2 || corrArr[widthNr, heightNr] == 2) ? 2 : 0;
            }
        }

        //create walls
        if (generateWalls)
        {
            for (int widthNr = 0; widthNr < dungeonWidh * amountOfRooms; widthNr++)
            {
                for (int heightNr = 0; heightNr < dungeonHeight * amountOfRooms; heightNr++)
                {
                    if (worldMap[widthNr, heightNr] == 2)
                     {
                         if (worldMap[widthNr + 1, heightNr] == 0)
                         {
                             worldMap[widthNr + 1, heightNr] = 1;
                         }
                         if (worldMap[widthNr - 1, heightNr] == 0)
                         {
                             worldMap[widthNr - 1, heightNr] = 1;
                         }
                         if (worldMap[widthNr, heightNr + 1] == 0)
                         {
                             worldMap[widthNr, heightNr + 1] = 1;
                         }
                         if (worldMap[widthNr, heightNr - 1] == 0)
                         {
                             worldMap[widthNr, heightNr - 1] = 1;
                         }
                     }
                }
            }
        }

        for (int widthNr = 0; widthNr < dungeonWidh * amountOfRooms; widthNr++)
        {
            for (int heightNr = 0; heightNr < dungeonHeight * amountOfRooms; heightNr++)
            {
                if (worldMap[widthNr, heightNr] == 0) continue;

                GameObject go = worldMap[widthNr, heightNr] == 1 ? wallTile :
                    worldMap[widthNr, heightNr] == 2 ? floorTile :
                    wallTile;

                Vector3 offset = go.transform.position;

                go = Instantiate(go);
                go.transform.position = new Vector3(widthNr, 0, heightNr) + offset;
                go.transform.parent = oldHolder.transform;
            }
        }
                dungeonElementPlacement.NewDungeon();
        setWorldArray();
    }

    public void GenerateCompleteDungeon(bool inGame = false)
    {
        if(inGame)
        {
            dungeonElementPlacement.RemovElementsInGame();
        }
        GenerateDungeon(true, inGame);
        dungeonElementPlacement.CompleteDungeon();
        setWorldArray();
    }

    public void RemoveWalls()
    {

        temp();
        for (int i = oldHolder.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(oldHolder.transform.GetChild(i).gameObject);
        }

        //remove walls
        for (int widthNr = 1; widthNr < dungeonWidh * amountOfRooms - 1; widthNr++)
        {
            for (int heightNr = 1; heightNr < dungeonHeight * amountOfRooms - 1; heightNr++)
            {
                if (worldMap[widthNr, heightNr] == 1)
                {
                    worldMap[widthNr, heightNr] = 0;
                }
            }
        }


        //buildRoom
        //göra en koridor peice som korridoerer kan sättas till (ska göras, i loppen skall sedan man kolla om det hör till en korridor isf )
        for (int roomMatrix = 0; roomMatrix < closedPosition.Count; roomMatrix++)
        {
            GameObject newRoom = new GameObject("room " + (roomMatrix + 1).ToString());
            newRoom.transform.parent = oldHolder.transform;
            for (int widthNr = (int)closedPosition[roomMatrix].from.x; widthNr < (int)closedPosition[roomMatrix].to.x; widthNr++)
            {
                for (int heightNr = (int)closedPosition[roomMatrix].from.z; heightNr < (int)closedPosition[roomMatrix].to.z; heightNr++)
                {
                    if (worldMap[widthNr, heightNr] == 0) continue;

                    GameObject go = worldMap[widthNr, heightNr] == 1 ? wallTile :
                        worldMap[widthNr, heightNr] == 2 ? floorTile :
                        wallTile;

                    Vector3 offset = go.transform.position;

                    go = Instantiate(go);
                    go.transform.position = new Vector3(widthNr, 0, heightNr) + offset;
                    go.transform.parent = newRoom.transform;
                }
            }
        }
        dungeonElementPlacement.NewDungeon();
        setWorldArray();
    }
    public void AddWalls()
    {
        temp();
        for (int i = oldHolder.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(oldHolder.transform.GetChild(i).gameObject);
        }
        //create walls
        for (int widthNr = 1; widthNr < dungeonWidh * amountOfRooms - 1; widthNr++)
            {
                for (int heightNr = 1; heightNr < dungeonHeight * amountOfRooms - 1; heightNr++)
                {
                    if (worldMap[widthNr, heightNr] == 2)
                    {
                        if (worldMap[widthNr + 1, heightNr] == 0)
                        {
                            worldMap[widthNr + 1, heightNr] = 1;
                        }
                        if (worldMap[widthNr - 1, heightNr] == 0)
                        {
                            worldMap[widthNr - 1, heightNr] = 1;
                        }
                        if (worldMap[widthNr, heightNr + 1] == 0)
                        {
                            worldMap[widthNr, heightNr + 1] = 1;
                        }
                        if (worldMap[widthNr, heightNr - 1] == 0)
                        {
                            worldMap[widthNr, heightNr - 1] = 1;
                        }
                    }
                    
            }
            }
        


        //buildRoom
        //göra en koridor peice som korridoerer kan sättas till (ska göras, i loppen skall sedan man kolla om det hör till en korridor isf )
        for (int roomMatrix = 0; roomMatrix < closedPosition.Count; roomMatrix++)
        {
            GameObject newRoom = new GameObject("room " + (roomMatrix + 1).ToString());
            newRoom.transform.parent = oldHolder.transform;
            for (int widthNr = (int)closedPosition[roomMatrix].from.x; widthNr < (int)closedPosition[roomMatrix].to.x; widthNr++)
            {
                for (int heightNr = (int)closedPosition[roomMatrix].from.z; heightNr < (int)closedPosition[roomMatrix].to.z; heightNr++)
                {
                    if (worldMap[widthNr, heightNr] == 0) continue;

                    GameObject go = worldMap[widthNr, heightNr] == 1 ? wallTile :
                        worldMap[widthNr, heightNr] == 2 ? floorTile :
                        wallTile;

                    Vector3 offset = go.transform.position;

                    go = Instantiate(go);
                    go.transform.position = new Vector3(widthNr, 0, heightNr) + offset;
                    go.transform.parent = newRoom.transform;
                }
            }
        }
        dungeonElementPlacement.NewDungeon();
        setWorldArray();
    }


    private void OnDrawGizmos()
    {
        if (drawGizmozCorridors)
        {
            Gizmos.color = Color.cyan;
            foreach (CorridorBuildStruct cor in corridorsLeftToBuild)
            {
                Gizmos.DrawLine((cor.fromPos + new Ivec3(0, 1, 0)), (cor.middlePos + new Ivec3(0, 1, 0)));
                Gizmos.DrawLine(cor.middlePos + new Ivec3(0, 1, 0), cor.toPos + new Ivec3(0, 1, 0));
                Gizmos.DrawLine(cor.middlePos + new Ivec3(0, 1, 0), cor.middlePos);
            }
        }

        Ivec3 offset = new Ivec3(dungeonWidh / 2, 0, dungeonHeight / 2);
        for (int i = 0; i < closedPosition.Count; i++)
        {
            if (drawGizmozMain)
            {
                if (closedPosition[i].isMain)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.blue;
                }
                Gizmos.DrawSphere(closedPosition[i].from + offset, 1);
            }
            if (drawGizmozDirection)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere((Vector3)closedPosition[closedPosition[i].fromId].from + (Vector3)offset + new Vector3(0, 1 + i / 5, 0), 0.5f);
                Gizmos.DrawLine((Vector3)closedPosition[closedPosition[i].fromId].from + (Vector3)offset + new Vector3(0, 1 + i / 5, 0), closedPosition[i].from + offset + new Ivec3(0, 1 + i / 5, 0));
            }

        }
        if (drawGizmozDisplayRoom)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere((Vector3)closedPosition[roomDisplayIndex].from + (Vector3)offset, 1.5f);
        }
    }
}
