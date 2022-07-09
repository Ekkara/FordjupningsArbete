using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineScript : DungeonElement, IDestructibleObjectBase
{
    [SerializeField] float collideDamage;
    [SerializeField] GameObject onDestroyParticles;
    [SerializeField] AudioClip[] ExplosionSound;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            IDestructibleObjectBase CBS = other.GetComponent<IDestructibleObjectBase>();
            CBS.takeDamage(collideDamage);
            OnDeath();

        }
    }
    public void takeDamage(float dmg, Vector3 damageDirection = new Vector3())
    {
        OnDeath();
    }
    public void OnDeath()
    {
       GameObject particleEffect = Instantiate(onDestroyParticles, transform.position, transform.rotation);
        particleEffect.transform.parent = DGS.oldHolder.transform;
        PlayerScript.playSoundEffect(ExplosionSound[Random.Range(0, ExplosionSound.Length - 1)], transform.position, 0.8f);
        Destroy(gameObject);
    }
}
