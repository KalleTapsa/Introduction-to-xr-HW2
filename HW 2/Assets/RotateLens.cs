using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Made so that lens rotate works correctly
public class RotateLens : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float zRotation = transform.parent.localEulerAngles.z;
            
        
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -zRotation);
        
    }
}
