using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestructibleObjectBase 
{
    void takeDamage(float damage, Vector3 damageDirection = new Vector3());
    void OnDeath();
}
/*
[SerializeField] float startHealth;
public float currentHealth;
// Start is called before the first frame update

virtual public void OnStart()
{
    currentHealth = startHealth;
}

virtual public void takeDamage(float damage)
{
    currentHealth -= damage;
    if (currentHealth <= 0)
    {
        OnDeath();
    }
}
virtual public void OnDeath()
{
    Destroy(gameObject);
}
*/