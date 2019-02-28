using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject maze;
    public float xRotation, zRotation;
    public float rotationSpeed = 0.3f;
    public float maxAngle = 5;

    // Start is called before the first frame update
    void Start()
    {
        xRotation = zRotation = 0;
    }

    // Update is called once per frame
    void Update()
    {
       
           
        zRotation -= Input.GetAxis("Horizontal") * rotationSpeed;
        xRotation += Input.GetAxis("Vertical") * rotationSpeed;
        zRotation = Mathf.Clamp(zRotation, -maxAngle, maxAngle);
        xRotation = Mathf.Clamp(xRotation, -maxAngle, maxAngle);
        maze.transform.rotation = Quaternion.Euler(xRotation, 0, zRotation);
    }
}
