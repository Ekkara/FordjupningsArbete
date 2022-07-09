using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake
{
    public float timeP;
    public float effectStrenght;
}
public class ScreenEffectScript : MonoBehaviour
{
    private static ScreenEffectScript _instance;
    public static ScreenEffectScript Instance
    {
        get { return _instance; }
    }


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogWarning("multiple singeltons of screenEffectScript are in the same scene!");
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    Vector3 restPosition;
    List<Shake> shakes = new List<Shake>();
    void Start()
    {
        restPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(shakes.Count > 0)
        {
            transform.localPosition = restPosition;
            float totalEffect = 0;
            for (int i = 0; i < shakes.Count; i++)
            {
               totalEffect += shakes[i].effectStrenght;
            }
            transform.localPosition += new Vector3(Random.Range(-totalEffect, totalEffect), 0, Random.Range(-totalEffect, totalEffect));
            for(int i = shakes.Count - 1; i >= 0; i--)
            {
                shakes[i].timeP -= Time.deltaTime;
                if(shakes[i].timeP <= 0)
                {
                    shakes.RemoveAt(i);
                    if(shakes.Count == 0)
                    {
                        transform.localPosition = restPosition;
                    }
                }
            }
        }
    }
    public void screenShake(float timePeriod, float effectStrength)
    {
        Shake newShake = new Shake();
        newShake.effectStrenght = effectStrength;
        newShake.timeP = timePeriod;
        shakes.Add(newShake);
    }
}
