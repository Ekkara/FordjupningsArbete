using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyScript : DungeonElement, IDestructibleObjectBase
{
    [SerializeField] float startHealth;
    public float currentHealth;
    Rigidbody RB;
    [SerializeField] float movmentSpeed;

    public List<Vector2> pathwayToGo;
    bool seePlayer = false;
    [SerializeField] float visionRadius = 12;
    [SerializeField] float acceptanseRange =  1;
    [SerializeField] HealthBarScript healthBar;
    [SerializeField] float damageToPlayer = 1;
    [SerializeField] GameObject onDestroyParticles;
    [SerializeField] AudioClip[] ExplosionSound;
    AudioSource audioSource;


    public Ivec2 playerPos = new Ivec2();
    void Start()
    {
        seePlayer = false;
        inPersue = false;
        healthBar.gameObject.SetActive(false);

        audioSource = gameObject.GetComponent<AudioSource>();

        healthBar.inisiateHealthBar(currentHealth, DGS.camera.transform);
        playerPos.x = PlayerScript.currentWidth;
        playerPos.y = PlayerScript.currentHeight;
        playerOldPos = PlayerScript.Pos;
        OnStart();
        RB = gameObject.GetComponent<Rigidbody>();
    }
    Vector3 playerOldPos;
    Vector3 moveDirection;
    Vector3 oldDirection;
    public bool inPersue = false;

    public bool isClose = false;

    void Update()
    {
        playerPos.x = PlayerScript.currentWidth;
        playerPos.y = PlayerScript.currentHeight;

      
        //widthPosition = (int)transform.position.x;
        //heightPosition = (int)transform.position.z;

      isClose = Vector3.Distance(PlayerScript.Pos, transform.position) <= visionRadius;
       // healthBar.gameObject.SetActive(inPersue);
        if (isClose)
        {
            RaycastHit hit = new RaycastHit();

            LayerMask levelMask = LayerMask.GetMask("Level");
            LayerMask playerMask = LayerMask.GetMask("Player", "Level");
            Physics.Raycast(transform.position, PlayerScript.Pos - transform.position, out hit, visionRadius, playerMask);

            if (hit.transform != null)
            {
               seePlayer = (hit.transform.tag == "Player");
                if(seePlayer)
                {
                    inPersue = true;
                    healthBar.gameObject.SetActive(true);

                }
            }

            if (inPersue)
            {
                RaycastHit floorHit = new RaycastHit();
                Physics.Raycast(transform.position, Vector3.down, out floorHit, 2, levelMask);
                if (floorHit.transform != null)
                {
                    widthPosition = (int)floorHit.transform.position.x;
                    heightPosition = (int)floorHit.transform.position.z;
                }
                if (seePlayer)
                {
                    if (playerPos.x == widthPosition || playerPos.y == heightPosition)
                    {
                        if (PlayerScript.Pos != playerOldPos)
                        {
                            
                            pathwayToGo = AlgorithmsScript.getPathAstar(
                                            new Ivec2(widthPosition, heightPosition),
                                            playerPos,
                                            DGS.dungeonWidh * DGS.amountOfRooms,
                                            DGS.dungeonHeight * DGS.amountOfRooms,
                                            DGS.worldMap,
                                            DGS.amountOfRooms);

                            playerOldPos = PlayerScript.Pos;
                 
                        }
                    }
                    else
                    {
                        pathwayToGo.Clear();
                        pathwayToGo.Add(new Vector2(PlayerScript.Pos.x, PlayerScript.Pos.z));
                    }
                }
                else
                {
                    
                    if (PlayerScript.Pos != playerOldPos)
                    {
                        
                        pathwayToGo = AlgorithmsScript.getPathAstar(
                                        new Ivec2(widthPosition, heightPosition),
                                        playerPos,
                                        DGS.dungeonWidh * DGS.amountOfRooms,
                                        DGS.dungeonHeight * DGS.amountOfRooms,
                                        DGS.worldMap,
                                        DGS.amountOfRooms);

                        playerOldPos = PlayerScript.Pos;

                    }
                }
            }
        }
        else
        {
            inPersue = false;
            healthBar.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
       if (inPersue)
        {
            if (seePlayer)
            {
                moveDirection = new Vector3(PlayerScript.Pos.x - transform.position.x, transform.position.y, PlayerScript.Pos.z - transform.position.z).normalized;

                transform.position = Vector3.MoveTowards(transform.position, PlayerScript.Pos +moveDirection, movmentSpeed * Time.deltaTime);
            }
            else if (pathwayToGo.Count > 0)
            {
                //moveDirection = new Vector3(pathwayToGo[0].x - transform.position.x, transform.position.y, pathwayToGo[0].y - transform.position.z).normalized;
                //RB.MovePosition(transform.position + moveDirection * movmentSpeed * Time.deltaTime);

                if (Vector3.Distance(new Vector3(pathwayToGo[0].x, transform.position.y, pathwayToGo[0].y), transform.position) <= acceptanseRange)
                {
                    pathwayToGo.RemoveAt(0);
                }
                if (pathwayToGo.Count > 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(pathwayToGo[0].x, transform.position.y, pathwayToGo[0].y), movmentSpeed * Time.deltaTime);
                }
            }
        }
     
    }
    


    virtual public void OnStart()
    {
        currentHealth = startHealth;
    }
    public void takeDamage(float damage, Vector3 damageDirection = new Vector3())
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            OnDeath();
        }
        else
        {
            RB.AddRelativeForce(damageDirection * 120);
            healthBar.setHealth(currentHealth);
        }
    }
    public void OnDeath()
    {
        GameObject particle = Instantiate(onDestroyParticles, transform.position, Quaternion.identity);
        particle.transform.parent = DGS.oldHolder.transform;
        PlayerScript.playSoundEffect(ExplosionSound[Random.Range(0, ExplosionSound.Length - 1)], transform.position, 0.8f);
        Destroy(gameObject);       
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, acceptanseRange/2);
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        if(pathwayToGo.Count > 0)
        {
            Gizmos.DrawLine(new Vector3(pathwayToGo[0].x, transform.position.y, pathwayToGo[0].y), transform.position);
        }
        for (int i = 0; i < pathwayToGo.Count; i++)
        {
            if (i < pathwayToGo.Count - 1)
            {
                Gizmos.DrawLine(new Vector3(pathwayToGo[i].x, transform.position.y, pathwayToGo[i].y), new Vector3(pathwayToGo[i + 1].x, transform.position.y, pathwayToGo[i + 1].y));
            }
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + moveDirection);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
            player.takeDamage(damageToPlayer);
            OnDeath();
        }
    }
}
