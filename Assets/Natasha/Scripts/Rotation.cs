using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public Vector3 vec;
    // public Vector3 axis = Vector3.forward;
    // public float speed = 1f;
    //GameObject object;
    // Start is called before the first frame update
    void Start()
    {
       // object = 
    }

    // Update is called once per frame
    void Update()
    {
        // gameObject.transform.eulerAngles += vec;
        gameObject.transform.Rotate(vec);
    }
}
