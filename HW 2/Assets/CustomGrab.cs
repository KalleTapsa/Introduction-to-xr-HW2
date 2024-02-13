using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// This class handles the grabbing and manipulation of objects in a virtual reality environment. Got a lot of help from ChatGPT since I was stuck at doing bachelor's thesis

public class CustomGrab : MonoBehaviour
{
    CustomGrab otherHand = null;
    public List<Transform> nearObjects = new List<Transform>();
    public Transform grabbedObject = null;
    public InputActionReference action;
    public InputActionReference doubleRotationAction;
    bool grabbing = false;
    private Vector3 prevPosition;
    private Quaternion prevRotation;
    public bool doubleRotation = false; // Toggle this via the Inspector or programmatically

    private void Start()
    {
        action.action.Enable();

        doubleRotationAction.action.Enable();

        doubleRotationAction.action.performed += (ctx) =>
        {
            doubleRotation = !doubleRotation;
        };

        foreach (CustomGrab c in transform.parent.GetComponentsInChildren<CustomGrab>())
        {
            if (c != this)
                otherHand = c;
        }
        prevPosition = transform.position;
        prevRotation = transform.rotation;
    }

    void Update()
    {
        grabbing = action.action.IsPressed();
        if (grabbing) {
            if (!grabbedObject) {
                grabbedObject = nearObjects.Count > 0 ? nearObjects[0] : otherHand.grabbedObject;
            }
            
            if (grabbedObject) {
                // Calculate delta position and rotation
                Vector3 deltaPosition = transform.position - prevPosition;
                Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(prevRotation);
                // If the other hand is not grabbing, apply the delta position and rotation to the grabbed object
                if(!otherHand.grabbing) {
                    grabbedObject.position += deltaPosition;
                // else if other hand is grabbing the same object, average the delta position and rotation for both hands
                } else if (otherHand.grabbedObject == grabbedObject && otherHand.grabbing) {
                    Vector3 combinedDelta = deltaPosition + (otherHand.transform.position - otherHand.prevPosition);
                // average the delta position when both hands are grabbing the same object
                    grabbedObject.position += combinedDelta * 0.5f;

                    Quaternion combinedRotation = deltaRotation * Quaternion.Inverse(otherHand.prevRotation) * otherHand.transform.rotation;

                    deltaRotation = combinedRotation;
                }
                // If doubleRotation is enabled, rotate the object twice as much
                if (doubleRotation) {
                deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
                angle = angle * 2.0f;
                deltaRotation = Quaternion.AngleAxis(angle, axis);
                }
                // Apply the rotation to the grabbed object
                grabbedObject.rotation = deltaRotation * grabbedObject.rotation;
                // checks if there is a change in position or rotation and applies it to the grabbed object
                if(deltaPosition != Vector3.zero || deltaRotation != Quaternion.identity) {
                    Vector3 dirToController = grabbedObject.position - transform.position;
                    dirToController = deltaRotation * dirToController;
                    grabbedObject.position = transform.position + dirToController;
                }
                    
            }
    }
    else if (grabbedObject)
    {
        grabbedObject = null;
    }
        // Save the current position and rotation for the next frame
        prevPosition = transform.position;
        prevRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform t = other.transform;
        if (t && t.CompareTag("Grabbable"))
            nearObjects.Add(t);
    }

    private void OnTriggerExit(Collider other)
    {
        Transform t = other.transform;
        if (t && t.CompareTag("Grabbable"))
            nearObjects.Remove(t);
    }
}