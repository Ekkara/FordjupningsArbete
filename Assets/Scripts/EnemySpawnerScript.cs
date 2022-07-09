using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : DungeonElement
{
    [SerializeField] GameObject Enemy;
    [SerializeField] float spawnYOffset = 1;
    [SerializeField] float minGenerationTime, maxGenerationTime, currentgenerationTime, playerRange;
    bool isEnemySpawned = false;
    GameObject enemy;
    
        float counter;

    // Start is called before the first frame update
    void Start()
    {
        GenerateEnemy();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnemySpawned)
        {
            if (enemy == null)
            {
                isEnemySpawned = false;
            }

        }
        else
        {
            if (Vector3.Distance(PlayerScript.Pos, transform.position) <= playerRange)
            {
                counter += Time.deltaTime;
                if (counter >= currentgenerationTime)
                {
                    GenerateEnemy();
                    counter = 0;
                }
            }
        }
    }
    void GenerateEnemy()
    {
        GameObject newEnemy = Instantiate(Enemy, transform.position + (Vector3.up * spawnYOffset), Quaternion.identity);
        newEnemy.transform.SetParent(DGS.oldHolder.transform);

        DungeonElement DE = newEnemy.GetComponent<DungeonElement>();
        DE.DGS = DGS;
        DE.testMap = (int[,])DGS.worldMap.Clone();

        currentgenerationTime = Random.Range(minGenerationTime, maxGenerationTime);
        isEnemySpawned = true;
        enemy = newEnemy;
    }
}
