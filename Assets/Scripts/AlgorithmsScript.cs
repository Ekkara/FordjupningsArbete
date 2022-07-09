using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    //h is estimated cost
    //g is the current value
    //f is the total
    
    public float g, h, f;
    public Path previous;
    public Ivec2 pos;
}

public class AlgorithmsScript : MonoBehaviour
{
    static bool pathExistInList(List<Path> pathList, Ivec2 pos)
    {
        foreach(Path path in pathList)
        {
            if(path.pos.Equals(pos))
            {
                return true;
            }
        }
        return false;
    }
    static Path getPath(List<Path> pathList, Ivec2 pos)
    {
        foreach(Path path in pathList)
        {
            if(path.pos.Equals(pos))
            {
                return path;
            }
        }
        //Debug.LogError("error 404 path not found");
        return null;
    }
    /*public static List<Path> getAddNeighbour(Path currentPath, List<Path> closedList, List<Path> openList, int width, int height, int mapWidth, int mapHeight)
    {
        List<Path> newList = openList;
        if ((0 <= width && width <= mapWidth) && (0 <= height && height <= mapHeight))
        {
            if (!pathExistInList(closedList, new Vector2(width, height)))
            {
                currentPath
            }
        }
        return newList;
    }*/
        public static List<Vector2> getPathAstar(Ivec2 startPos, Ivec2 targetPos, int mapWidth, int mapHeight, int[,] worldMap, int amountOfRooms)
    {
        List<Vector2> currentPath = new List<Vector2>();
        List<Path> openList = new List<Path>();
        List<Path> closedList = new List<Path>();
        Path current = new Path();

        Path init = new Path();
        init.pos = startPos;
        init.g = 0;
        init.h = Ivec2.Distance(startPos, targetPos);
        init.f = init.g + init.h;
        openList.Add(init);

        while(openList.Count > 0)
        {
            float comp = openList[0].f;

            for (int i = 0; i < openList.Count; i++)
            {
                if (comp >= openList[i].f)
                {
                    comp = openList[i].f;
                    current = openList[i];
                }
            }
            openList.Remove(current);
            closedList.Add(current);

            if(current.pos.Equals(targetPos))
            {
                if (!startPos.Equals(targetPos))
                {
                    while (true)
                    {
                        currentPath.Add(current.pos);
                        if (current.previous != null)
                        {
                            current = current.previous;
                        }
                        else
                        {
                            break;
                        }
                    }
                    currentPath.Reverse();
                }
                return currentPath;
            }

           
            //lol this is needed 8times :)
            #region up
            //c
            if (current.pos.y < mapHeight * amountOfRooms)//c
            {
                //Debug.Log(worldMap[current.pos.x, current.pos.y + 1]);
                if (worldMap[current.pos.x, current.pos.y + 1] == 2 
                    && getPath(closedList, new Ivec2(current.pos.x, current.pos.y + 1)) == null) //c //!pathExistInList(closedList, getPath(current.GC.up, closedList))
                {
                    Path thisPath;
                    if (pathExistInList(openList, new Ivec2(current.pos.x, current.pos.y + 1)))//c
                    {
                        thisPath = getPath(openList, new Ivec2(current.pos.x, current.pos.y + 1));//c

                        if (thisPath.h > current.h + 1)
                        {
                            for (int i = 0; i < openList.Count; i++)
                            {
                                if (openList[i].pos.Equals(thisPath.pos))
                                {
                                    openList[i].h = current.h + 1;
                                    openList[i].g = Vector2.Distance(targetPos, openList[i].pos);
                                    openList[i].f = openList[i].g + openList[i].h;
                                    openList[i].previous = current;
                                    break;
                                }
                            }

                        }
                    }
                    else
                    {
                        thisPath = new Path();
                        thisPath.h = current.h + 1;
                        thisPath.pos = current.pos + new Ivec2(0,1);//c
                        thisPath.g = Ivec2.Distance(targetPos, thisPath.pos);
                        thisPath.f = thisPath.h + thisPath.g;
                        thisPath.previous = current;
                        openList.Add(thisPath);
                    }
                }
            }
            #endregion
            #region down
            //c
            if (0 < current.pos.y)//c
            {
                if (worldMap[current.pos.x, current.pos.y - 1] == 2 && getPath(closedList, new Ivec2(current.pos.x, current.pos.y - 1)) == null) //c //!pathExistInList(closedList, getPath(current.GC.up, closedList))
                {
                    Path thisPath;
                    if (pathExistInList(openList, new Ivec2(current.pos.x, current.pos.y - 1)))//c
                    {
                        thisPath = getPath(openList, new Ivec2(current.pos.x, current.pos.y - 1));//c

                        if (thisPath.h > current.h + 1)
                        {
                            for (int i = 0; i < openList.Count; i++)
                            {
                                if (openList[i].pos.Equals(thisPath.pos))
                                {
                                    openList[i].h = current.h + 1;
                                    openList[i].g = Ivec2.Distance(targetPos, openList[i].pos);
                                    openList[i].f = openList[i].g + openList[i].h;
                                    openList[i].previous = current;
                                    break;
                                }
                            }

                        }
                    }
                    else
                    {
                        thisPath = new Path();
                        thisPath.h = current.h + 1;
                        thisPath.pos = current.pos + new Ivec2(0,-1);//c
                        thisPath.g = Ivec2.Distance(targetPos, thisPath.pos);
                        thisPath.f = thisPath.h + thisPath.g;
                        thisPath.previous = current;
                        openList.Add(thisPath);
                    }
                }
            }
            #endregion
            #region Right
            //c
            if (current.pos.x < mapWidth * amountOfRooms)//c
            {
                if (worldMap[current.pos.x + 1, current.pos.y] == 2 &&
                    getPath(closedList, new Ivec2(current.pos.x + 1, current.pos.y)) == null) //c //!pathExistInList(closedList, getPath(current.GC.up, closedList))
                {
                    
                    Path thisPath;
                    if (pathExistInList(openList, new Ivec2(current.pos.x + 1, current.pos.y)))//c
                    {
                        thisPath = getPath(openList, new Ivec2(current.pos.x + 1, current.pos.y));//c

                        if (thisPath.h > current.h + 1)
                        {
                            for (int i = 0; i < openList.Count; i++)
                            {
                                if (openList[i].pos.Equals(thisPath.pos))
                                {
                                    openList[i].h = current.h + 1;
                                    openList[i].g = Ivec2.Distance(targetPos, openList[i].pos);
                                    openList[i].f = openList[i].g + openList[i].h;
                                    openList[i].previous = current;
                                    break;
                                }
                            }

                        }
                    }
                    else
                    {
                        thisPath = new Path();
                        thisPath.h = current.h + 1;
                        thisPath.pos = current.pos + new Ivec2(1, 0);//c
                        thisPath.g = Ivec2.Distance(targetPos, thisPath.pos);
                        thisPath.f = thisPath.h + thisPath.g;
                        thisPath.previous = current;
                        openList.Add(thisPath);
                    }
                }
            }
            #endregion
            #region Left
            //c
            if (0 < current.pos.x)//c
            {
                if (worldMap[current.pos.x - 1, current.pos.y] == 2 && getPath(closedList, new Ivec2(current.pos.x - 1, current.pos.y)) == null) //c //!pathExistInList(closedList, getPath(current.GC.up, closedList))
                {
                    Path thisPath;
                    if (pathExistInList(openList, new Ivec2(current.pos.x - 1, current.pos.y)))//c
                    {
                        thisPath = getPath(openList, new Ivec2(current.pos.x - 1, current.pos.y));//c

                        if (thisPath.h > current.h + 1)
                        {
                            for (int i = 0; i < openList.Count; i++)
                            {
                                if (openList[i].pos.Equals(thisPath.pos))
                                {
                                    openList[i].h = current.h + 1;
                                    openList[i].g = Ivec2.Distance(targetPos, openList[i].pos);
                                    openList[i].f = openList[i].g + openList[i].h;
                                    openList[i].previous = current;
                                    break;
                                }
                            }

                        }
                    }
                    else
                    {
                        thisPath = new Path();
                        thisPath.h = current.h + 1;
                        thisPath.pos = current.pos + new Ivec2(-1,0);//c
                        thisPath.g = Ivec2.Distance(targetPos, thisPath.pos);
                        thisPath.f = thisPath.h + thisPath.g;
                        thisPath.previous = current;
                        openList.Add(thisPath);
                    }
                }
            }
            #endregion
           

        }
        //return failure
        Debug.LogWarning("no path found");
        return currentPath;
    }
}
