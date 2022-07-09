using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarScript : DungeonElement
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            DGS.amountOfRooms++;
            Destroy(DGS.oldHolder);
            DGS.GenerateCompleteDungeon(true);
            menuScript.score++;
        }
    }
}
