using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] Vector3 rotation;
    [SerializeField] float speed;
    [SerializeField] Vector3 movementDirection;
    [SerializeField] float damage;
    [SerializeField] GameObject particleObject;
    [SerializeField] AudioClip[] onColideClipps;
    Transform sceneHolder;
    // Start is called before the first frame update
   
    public void setDirection(Vector3 direction, Transform sceneHolder)
    {
        movementDirection = direction;
        this.sceneHolder = sceneHolder;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation * Time.deltaTime);
        transform.position += movementDirection * speed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            IDestructibleObjectBase functionBase = other.GetComponent<IDestructibleObjectBase>();
            functionBase.takeDamage(damage, movementDirection);
            Destroy(gameObject);
            GenerateParticles(movementDirection);
            GenerateParticles(other.transform.position - transform.position);

            

            PlayerScript.playSoundEffect(onColideClipps[Random.Range(0, onColideClipps.Length - 1)], transform.position, 0.1f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Level")
        {
            Destroy(gameObject);
            GenerateParticles(movementDirection);
            GenerateParticles(collision.transform.position - transform.position);
            PlayerScript.playSoundEffect(onColideClipps[Random.Range(0, onColideClipps.Length - 1)], transform.position, 0.1f);
        }
    }
    void GenerateParticles(Vector3 direction)
    {
        GameObject particles = Instantiate(particleObject, transform.position, Quaternion.LookRotation(-direction));
        particles.transform.parent = sceneHolder.transform;

    }
    
}
