using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image fill;
    [SerializeField] Gradient gradient;
    Transform camara;
    [SerializeField] GameObject parant;

    public void inisiateHealthBar(float startHealth, Transform camerasTrans)
    {
        camara = camerasTrans;
        slider.maxValue = startHealth;
        setHealth(startHealth);
    }

    public void setHealth(float health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    private void LateUpdate()
    {
        parant.transform.LookAt(parant.transform.position + camara.forward);
    }
}
