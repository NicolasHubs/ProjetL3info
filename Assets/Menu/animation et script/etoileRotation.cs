using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class etoileRotation : MonoBehaviour {


    public float rotateX = 1.0f;
    public float rotateY = 0f;
    public float rotateZ = 0f;
	
	// Update is called once per frame
	void Update () {
		if (rotateX > 0)
        {
            transform.Rotate(rotateX * Time.deltaTime , 0f, 0f); 
        }
        if (rotateY > 0)
        {
            transform.Rotate(0f, rotateY * Time.deltaTime, 0f);
        }
        if (rotateZ > 0)
        {
            transform.Rotate( 0f, 0f, rotateZ * Time.deltaTime );
        }
    }
}
