using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerScript : MonoBehaviour, IDestructibleObjectBase
{
    public static int currentWidth, currentHeight;
    public static Vector3 Pos;
    Rigidbody RB;
    [SerializeField] float movmentSpeed;
    [SerializeField] DungeonsGeneratorScript DGS;
    [SerializeField] GameObject projectile;
    [SerializeField] Camera camera;
    [SerializeField] AudioClip[] shoot;
    static AudioSource audioSource;
    [SerializeField] float fireReloadTime;
    [SerializeField] HealthBarScript healthBar;

    private void Awake()
    {
        currentWidth = (int)transform.position.x;
        currentHeight = (int)transform.position.z;
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        audioSource = gameObject.GetComponent<AudioSource>();
        OnStart();
        RB = gameObject.GetComponent<Rigidbody>();
        healthBar.inisiateHealthBar(currentHealth, camera.transform);
        DGS.GenerateCompleteDungeon(true);
    }

    bool hasFired = false;
    Vector3 moveDirection;
    // Update is called once per frame
    void Update()
    {
        //Debug.Log((int)(1f / Time.unscaledDeltaTime));
        Pos = transform.position;
        moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector3.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector3.left;
        }
        moveDirection = moveDirection.normalized;

        if(Input.GetMouseButton(0) && !hasFired)
        {
            GameObject newProj = Instantiate(projectile, transform.position, transform.rotation);
            ProjectileScript projScript = newProj.gameObject.GetComponent<ProjectileScript>();
            newProj.transform.parent = DGS.oldHolder.transform;

              Vector3 mousePos = Input.mousePosition;
            Plane plane = new Plane(Vector3.up, 0);

            float distance;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (plane.Raycast(ray, out distance))
            {
                mousePos = ray.GetPoint(distance);
                mousePos.y = transform.position.y;
            }
            Vector3 fireDirection = (mousePos - transform.position).normalized;
            projScript.setDirection(fireDirection, DGS.oldHolder.transform);
            playSoundEffect(shoot[Random.Range(0, shoot.Length - 1)]);
            hasFired = true;
            Invoke("resetFire", fireReloadTime);

            RB.velocity = Vector3.zero;
            RB.AddRelativeForce(-fireDirection * 100);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            DGS.GenerateCompleteDungeon();
        }
        //currentWidth = (int)transform.position.x;
        //currentHeight = (int)transform.position.z;

        RaycastHit hit = new RaycastHit();
        LayerMask mask = LayerMask.GetMask("Level");
        Physics.Raycast(transform.position, Vector3.down, out hit, 2, mask);
        if(hit.transform != null)
        {
            currentWidth = (int)hit.transform.position.x;
            currentHeight = (int)hit.transform.position.z;
        }
    }
    void resetFire()
    {
        hasFired = false;
    }
    [SerializeField] float startHealth;
    public float currentHealth;

    virtual public void OnStart()
    {
        currentHealth = startHealth;
    }
    public static void playSoundEffect(AudioClip clip, float volume = 1)
    {
        audioSource.PlayOneShot(clip, volume);
    }
    public static void playSoundEffect(AudioClip clip, Vector3 pos, float volume = 1)
    {
        AudioSource.PlayClipAtPoint(clip, pos, volume);
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
            ScreenEffectScript.Instance.screenShake(0.286f, damage/6);
            healthBar.setHealth(currentHealth);
        }
    }
    public void OnDeath()
    {
        SceneManager.LoadScene("MenuScene");
    }

    private void FixedUpdate()
    {   
        RB.MovePosition(transform.position += moveDirection * movmentSpeed * Time.fixedDeltaTime);
    }

    public Texture2D cursorTexture;
}
