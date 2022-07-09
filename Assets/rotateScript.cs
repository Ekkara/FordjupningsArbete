using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateScript : MonoBehaviour
{
    [SerializeField] [Range(0,1)] int xRot, yRot, zRot;
    [SerializeField] float rotSpeed;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(xRot, yRot, zRot) * rotSpeed * Time.deltaTime);
    }
}
