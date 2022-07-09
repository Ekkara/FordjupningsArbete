using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Element
{
    public string Name;
    [SerializeField] [HideInInspector] public List<GameObject> placedElements = new List<GameObject>();


    [HideInInspector] public bool Use;
    [HideInInspector] public GameObject GameObject;
    [HideInInspector] public DungeonElementPlacement.PlacementType PlacementType;
    [HideInInspector] public float ySpawnOffset;
    //random per tile
    [HideInInspector] public float precentageChange;


    [HideInInspector] public bool lastMainRoom;
    [HideInInspector] public bool allowdInStartRoom;
    [HideInInspector] public int roomNr;
}


public class DungeonElementPlacement : MonoBehaviour
{
    private void OnValidate()
    {

    }
    public enum PlacementType
    {
        randomPerTile,
        CenterOfRoom
    }

    public Element[] elements;

    [SerializeField] [HideInInspector] List<Vector2> floorPositions = new List<Vector2>();
    [SerializeField] [HideInInspector] List<Vector2> wallPosition = new List<Vector2>();

    [SerializeField] DungeonsGeneratorScript dungenGenerator;


    public void RemoveAllPlacement(bool fullClean = true)
    {
        for (int i = 0; i < elements.Length; i++)
        {
            RemoveElement(i, fullClean);
        }
    }
    public void RemovElementsInGame()
    {
        for (int index = 0; index < elements.Length; index++)
        {
            for (int i = elements[index].placedElements.Count - 1; i >= 0; i--)
            {
                Destroy(elements[index].placedElements[i]);
                elements[index].placedElements.RemoveAt(i);
            }
        }
    }

    public void CompleteDungeon()
    {
        NewDungeon();
        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i].Use)
            {
                PlaceElement(i);
            }
        }
    }
    bool[] positionTaken;
    public void NewDungeon()
    {

        RemoveAllPlacement();
        floorPositions.Clear();
        wallPosition.Clear();

        int widthStart = ((dungenGenerator.dungeonWidh / 2) * dungenGenerator.amountOfRooms);
        int heightStart = (dungenGenerator.dungeonHeight / 2) * dungenGenerator.amountOfRooms;

        //update positions for the tiles
        for (int widthNr = 0; widthNr < dungenGenerator.dungeonWidh * dungenGenerator.amountOfRooms; widthNr++)
        {
            for (int heightNr = 0; heightNr < dungenGenerator.dungeonHeight * dungenGenerator.amountOfRooms; heightNr++)
            {
                if (dungenGenerator.worldMap[widthNr, heightNr] == 1)//if(dungenGenerator.worldMap[widthNr, heightNr] == 1)
                {
                    wallPosition.Add(new Vector2(widthNr, heightNr));
                }
                else if (dungenGenerator.worldMap[widthNr, heightNr] == 2)
                {
                    /*  if ((widthNr % widthStart != 0 || heightNr % heightStart != 0) &&
                          (widthNr % widthStart != 0 || (heightNr - 1) % (heightStart) != 0) &&
                          ((widthNr - 1) % widthStart != 0 || (heightNr - 1) % heightStart != 0) &&
                          ((widthNr - 1) % widthStart != 0 || heightNr % heightStart != 0))*/
                    {
                        floorPositions.Add(new Vector2(widthNr, heightNr));
                    }
                }
            }
        }
        positionTaken = new bool[floorPositions.Count];
    }

    public void PlaceElement(int index)
    {
        Debug.Log(elements[index].allowdInStartRoom);
        switch (elements[index].PlacementType)
        {
            case PlacementType.randomPerTile:
                
                for (int i = 0; i < floorPositions.Count; i++)
                {
                    if (!elements[index].allowdInStartRoom)
                    {
                        if (isInFirsRoom(floorPositions[i]))
                        {
                            continue;
                        }
                    }
                    float thisRandom = Random.RandomRange(0, 100);
                    if (thisRandom < elements[index].precentageChange && !positionTaken[i])
                    {
                        GameObject newElement = Instantiate(elements[index].GameObject);
                        newElement.transform.position = new Vector3(floorPositions[i].x, elements[index].ySpawnOffset, floorPositions[i].y);
                        elements[index].placedElements.Add(newElement);
                        newElement.transform.SetParent(dungenGenerator.oldHolder.transform);

                        DungeonElement DE = newElement.GetComponent<DungeonElement>();
                        DE.widthPosition = (int)floorPositions[i].x;
                        DE.heightPosition = (int)floorPositions[i].y;
                        DE.DGS = dungenGenerator;
                        DE.testMap = (int[,])dungenGenerator.worldMap.Clone();

                        positionTaken[i] = true;
                        DE.spawnedIndex = new int[1];
                        DE.spawnedIndex[0] = i;
                        //dungenGenerator.worldMap;
                        // DE.testt.waaIt = (int[,])dungenGenerator.worldMap.Clone();//dungenGenerator.worldMap;
                    }
                }
                break;

            case PlacementType.CenterOfRoom:

                Vector3 offset = new Vector3((dungenGenerator.dungeonWidh / 2) - 1, 0, (dungenGenerator.dungeonHeight / 2) - 1);
                GameObject newElementCR = Instantiate(elements[index].GameObject);

                Vector3 spawnPos = (Vector3)dungenGenerator.closedPosition[elements[index].roomNr].from + offset;

                if (elements[index].lastMainRoom)
                {
                    for (int i = dungenGenerator.closedPosition.Count - 1; i >= 0; i--)
                    {
                        if (dungenGenerator.closedPosition[i].isMain)
                        {
                            spawnPos = (Vector3)dungenGenerator.closedPosition[i].from + offset;
                            newElementCR.transform.position = spawnPos;

                            break;
                        }
                    }
                }
                else
                {
                    spawnPos = (Vector3)dungenGenerator.closedPosition[elements[index].roomNr].from + offset;
                    newElementCR.transform.position = spawnPos;
                }


                newElementCR.transform.SetParent(dungenGenerator.oldHolder.transform);

                DungeonElement DECr = newElementCR.GetComponent<DungeonElement>();
                DECr.DGS = dungenGenerator;
                DECr.testMap = (int[,])dungenGenerator.worldMap.Clone();

                Vector2 inisatePos = new Vector2(spawnPos.x, spawnPos.z);
                for (int i = 0; i < floorPositions.Count; i++)
                {
                    if (inisatePos == floorPositions[i])
                    {
                        changeBoolState(inisatePos, true);
                        changeBoolState(floorPositions[i] + new Vector2(1, 0), true);
                        changeBoolState(floorPositions[i] + new Vector2(0, 1), true);
                        changeBoolState(floorPositions[i] + new Vector2(1, 1), true);
                        break;
                    }
                }

                break;
        }
    }
    void changeBoolState(Vector2 pos, bool state)
    {
        for (int i = 0; i < floorPositions.Count; i++)
        {
            if (pos == floorPositions[i])
            {
                positionTaken[i] = state;
                break;
            }
        }
    }
    bool isInFirsRoom(Vector2 pos)
    {
        bool found;
        Vector2 from = new Vector2(dungenGenerator.startRoomBuildWidthOffset * dungenGenerator.dungeonWidh, dungenGenerator.startRoomBuildHeightOffset * dungenGenerator.dungeonHeight);
        Vector2 to = from + new Vector2(dungenGenerator.dungeonWidh, dungenGenerator.dungeonHeight);
        for(int i = 0; i < floorPositions.Count; i++)
        {
            if(pos.x > from.x && pos.y > from.y &&
                pos.x < to.x && pos.y < to.y)
            {
                return true;
            }
        }

        return false;
    }

    public void RemoveElement(int index, bool fullClean = false)
    {
        //for every element in the placed list, destroy them and remove them from the list
        for (int i = elements[index].placedElements.Count - 1; i >= 0; i--)
        {
            if (!fullClean)
            {
                DungeonElement DE = elements[index].placedElements[i].GetComponent<DungeonElement>();
                for (int j = 0; j < DE.spawnedIndex.Length; j++)
                {
                    positionTaken[elements[index].placedElements[i].GetComponent<DungeonElement>().spawnedIndex[j]] = false;
                }
                
            }
            DestroyImmediate(elements[index].placedElements[i]);
            elements[index].placedElements.RemoveAt(i);
        }
    }
}
