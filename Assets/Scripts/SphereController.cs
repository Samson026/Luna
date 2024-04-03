using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction.HandGrab;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class SphereController : MonoBehaviour
{
    public LibPdInstance pdPatch;
    public Transform leftHand;
    public Transform rightHand;
    private bool grabbed = false;
    private bool attached = false;
    private Vector3 gPosition;
    public HandGrabInteractable interactables;
    // Start is called before the first frame update
    void Start()
    {
        pdPatch.SendFloat("harm", 3.0f);
        pdPatch.SendFloat("note", 3.0f);
        pdPatch.SendFloat("vol", 0.1f);
        pdPatch.SendFloat("M", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if object is grabbed
        
        if (interactables.Interactors.Count > 0) {
            if (!grabbed) {
                grabbed = true;
            }
        }
        else {
            if (grabbed) {
                attached = !attached;
                grabbed = false;
                gPosition = transform.position;
            }
        }
       
        // pdPatch.SendFloat("ampmod", 1.0f);
        if (attached) {
            // send audio data



            // float leftHandDistance = Vector3.Distance(leftHand.position, gPosition);
            // leftHandDistance = leftHandDistance * 50;
            // float rightHandDistance = Vector3.Distance(rightHand.position, gPosition);
            // Debug.Log("right " + rightHandDistance);
            // Debug.Log("left " + leftHandDistance);
            // Debug.Log("pos" + gPosition);
            // pdPatch.SendFloat("M", rightHandDistance);
            // pdPatch.SendFloat("I", leftHandDistance);
            pdPatch.SendFloat("ampmod", Gap() * 100);
            pdPatch.SendFloat("note", Distance() * 8);
            pdPatch.SendFloat("I", Angle() * 50);

            // update position
            Vector3 pos = (rightHand.position + leftHand.position) /2;
            transform.position = pos;
        }
    }
    
    private float Gap() {
        return Vector3.Distance(leftHand.position, rightHand.position);
    }

    private float Angle() {
        float x = rightHand.position.x - leftHand.position.x;
        float y = rightHand.position.y - leftHand.position.y; 
        float z = rightHand.position.z - leftHand.position.z;

        // float mx = y/x;
        // float my = z/y;
        // float mz = z/x;
        // float t = System.MathF.Pow(mx, 2f) + System.MathF.Pow(my, 2f) + System.MathF.Pow(mz, 2f);
        float m = y/x;
        float angleRadians = Mathf.Atan(m);
        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        Debug.Log("angle degrees: " + angleDegrees);

        if (angleDegrees < 0)
            angleDegrees = MathF.Abs(angleDegrees);
        else
            angleDegrees += 40f;

        return angleDegrees/90;
    }

    private float Distance() {
        return Vector3.Distance((rightHand.position + leftHand.position) /2, gPosition);
    }


}
