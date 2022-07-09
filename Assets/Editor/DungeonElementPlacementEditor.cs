using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonElementPlacement))]
public class DungeonElementPlacementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DungeonElementPlacement DEP = (DungeonElementPlacement)target;
        base.OnInspectorGUI();
        if(DEP.elements.Length > 0)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Place all elements"))
            {
                for (int i = 0; i < DEP.elements.Length; i++)
                {
                    DEP.PlaceElement(i);
                }
            }
            if (GUILayout.Button("Remove all elements"))
            {
                DEP.RemoveAllPlacement(false);
            }
            GUILayout.EndHorizontal();
        }
        for(int i = 0; i < DEP.elements.Length; i++)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(DEP.elements[i].Name); 

            DEP.elements[i].Use = EditorGUILayout.Toggle("Use " + DEP.elements[i].Name, DEP.elements[i].Use);
            if (DEP.elements[i].Use)
            {
                DEP.elements[i].GameObject = (GameObject)EditorGUILayout.ObjectField(DEP.elements[i].Name + " game object:",DEP.elements[i].GameObject, typeof(GameObject));
                DEP.elements[i].ySpawnOffset = EditorGUILayout.FloatField("y spawn offset ", DEP.elements[i].ySpawnOffset);
                DEP.elements[i].PlacementType = (DungeonElementPlacement.PlacementType)EditorGUILayout.EnumPopup(DEP.elements[i].Name + " placement type", DEP.elements[i].PlacementType);

                if (DEP.elements[i].PlacementType == DungeonElementPlacement.PlacementType.randomPerTile)
                {
                    DEP.elements[i].allowdInStartRoom = EditorGUILayout.Toggle("Allow " + DEP.elements[i].Name + " in start room", DEP.elements[i].allowdInStartRoom);
                    DEP.elements[i].precentageChange = EditorGUILayout.Slider("precentage of spawning " + DEP.elements[i].Name, DEP.elements[i].precentageChange, 0, 100);
                }
                else if (DEP.elements[i].PlacementType == DungeonElementPlacement.PlacementType.CenterOfRoom)
                {
                    DEP.elements[i].lastMainRoom = EditorGUILayout.Toggle("place " + DEP.elements[i].Name + " in last room", DEP.elements[i].lastMainRoom);
                    if (!DEP.elements[i].lastMainRoom)
                    {
                        DEP.elements[i].roomNr = EditorGUILayout.IntField("room for placement of " + DEP.elements[i].Name, DEP.elements[i].roomNr);
                    }
                }
              /*  else
                {
                    DEP.elements[i].AmountMin = EditorGUILayout.IntField("min amount of " + DEP.elements[i].Name, DEP.elements[i].AmountMin);
                    DEP.elements[i].AmountMax = EditorGUILayout.IntField("max amount of " + DEP.elements[i].Name, DEP.elements[i].AmountMax);
                }*/
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Place " + DEP.elements[i].Name))
                {
                    DEP.PlaceElement(i);
                }
                if (GUILayout.Button("Remove all " + DEP.elements[i].Name +"s"))
                {
                    DEP.RemoveElement(i);
                }
                GUILayout.EndHorizontal();
            }
        }        
    }
}
