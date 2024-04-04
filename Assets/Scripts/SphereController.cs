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
    private Transform leftHandPos;
    private Transform rightHandPos;

    public OVRSkeleton leftHand;
    public OVRSkeleton rightHand;

    private bool grabbed = false;
    private bool attached = false;
    private Vector3 gPosition;
    private List<Vector3> positions;
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
        // get hand pos

        foreach (var bone in leftHand.Bones) {
            if (bone.Id == OVRSkeleton.BoneId.Hand_Thumb1) {
                leftHandPos = bone.Transform;
            }
        }

        foreach (var bone in rightHand.Bones) {
            if (bone.Id == OVRSkeleton.BoneId.Hand_Thumb1) {
                rightHandPos = bone.Transform;
            }
        }

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
       
        if (attached) {
            // send audio data

            pdPatch.SendFloat("ampmod", Gap() * 100);
            pdPatch.SendFloat("note", Distance() * 8);
            pdPatch.SendFloat("I", Angle() * 50);

            // update position
            Vector3 pos = (leftHandPos.position + rightHandPos.position) /2;
            positions.Add(pos);

            Debug.Log(positions.Count());

            if (positions.Count() >= 20) {
                transform.position = positions[0];
                positions.RemoveAt(0);
            }
        }
        else {
            positions = new List<Vector3>();
        }
    }
    
    private float Gap() {
        return Vector3.Distance(leftHandPos.position, rightHandPos.position);
    }

    private float Angle() {
        float x = rightHandPos.position.x - leftHandPos.position.x;
        float y = rightHandPos.position.y - leftHandPos.position.y; 
        float z = rightHandPos.position.z - leftHandPos.position.z;

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
        return Vector3.Distance((rightHandPos.position + leftHandPos.position) /2, gPosition);
    }


}
