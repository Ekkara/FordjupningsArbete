using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DungeonElement : MonoBehaviour
{
    public int widthPosition, heightPosition;
    public DungeonsGeneratorScript DGS;
    [SerializeField] public int[,] testMap;
    [HideInInspector] public int[] spawnedIndex;
}
