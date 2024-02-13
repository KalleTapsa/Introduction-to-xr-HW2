using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Made so that lens follows the camera
public class Follow : MonoBehaviour
{

    public Transform mainCameraTransform;

    public Transform lensTransform;

    void Start()
    {
    
    }

    void Update()
    {
        Vector3 LensDir = lensTransform.position - mainCameraTransform.position;
        transform.rotation = Quaternion.LookRotation(LensDir);
    }
}
