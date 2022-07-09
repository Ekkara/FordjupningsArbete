using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(DungeonsGeneratorScript))]
public class DungeonGeneratorEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        DungeonsGeneratorScript DGS = (DungeonsGeneratorScript)target;

        DGS.StartRoomPos = (DungeonsGeneratorScript.DeffineStartRoomPos)EditorGUILayout.EnumPopup("start room placment", DGS.StartRoomPos);

        if (DGS.StartRoomPos == DungeonsGeneratorScript.DeffineStartRoomPos.customOffsets)
        {
            DGS.startRoomBuildWidthOffset = EditorGUILayout.IntField("Dungen Width start pos", DGS.startRoomBuildWidthOffset);
            DGS.startRoomBuildHeightOffset = EditorGUILayout.IntField("Dungen height start pos", DGS.startRoomBuildHeightOffset);
        }
        else if (DGS.StartRoomPos == DungeonsGeneratorScript.DeffineStartRoomPos.middle)
        {
            DGS.startRoomBuildWidthOffset = DGS.amountOfRooms / 2;
            DGS.startRoomBuildHeightOffset = DGS.amountOfRooms / 2;
        }
        if(DGS.corridorType == CorridorType.tangledTree)
        {
            DGS.precantageOfBuild = EditorGUILayout.Slider("connectChance ", DGS.precantageOfBuild, 0, 100);
        }
        //normal functionalities for the editor
        base.OnInspectorGUI();


        //add a button
        if (GUILayout.Button("Generate Dungeon"))
        {
            DGS.GenerateDungeon();
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Remove Walls"))
        {
            DGS.RemoveWalls();
        }
        if (GUILayout.Button("Add Walls"))
        {
            DGS.AddWalls();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Generate complete dungeon"))
        {
            DGS.GenerateCompleteDungeon();
        }
    }
}

