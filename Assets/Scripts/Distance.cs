using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction.HandGrab;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Distance : MonoBehaviour
{
    public LibPdInstance pdPatch;
    public Transform leftHand;
    public Transform rightHand;
    private bool grabbed = false;
    private Vector3 gPosition;
    public string name;
    public HandGrabInteractor [] interactors;
    public HandGrabInteractable [] interactables;
    // Start is called before the first frame update
    void Start()
    {
        pdPatch.SendFloat("harm"+name, 3.0f);
        pdPatch.SendFloat("note"+name, 3.0f);
        pdPatch.SendFloat("vol", 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if object is grabbed
        bool aHand = false;
        // for (int i = 0; i < interactors.Length; i++) {
        //     if (interactors[i].State == Oculus.Interaction.InteractorState.Select) {
        //         aHand = true;
        //     }
        // }
         for (int i = 0; i < interactables.Length; i++) {
            if (interactables[i].Interactors.Count > 0) {
                aHand = true;
            }
        }

        if (aHand) {
            if (!grabbed) {
                grabbed = true;
                gPosition = transform.position;
            }
        }
        else
            grabbed = false;
       
        // pdPatch.SendFloat("ampmod", 1.0f);
        if (grabbed) {
            float leftHandDistance = Vector3.Distance(leftHand.position, gPosition);
            leftHandDistance = leftHandDistance * 50;
            float rightHandDistance = Vector3.Distance(rightHand.position, gPosition);
            // rightHandDistance = rightHandDistance;
            // if (rightHandDistance < 10)
            //     rightHandDistance = 1.0f;
            Debug.Log("right " + rightHandDistance);
            Debug.Log("left " + leftHandDistance);
            Debug.Log("pos" + gPosition);
            pdPatch.SendFloat("M"+name, rightHandDistance);
            pdPatch.SendFloat("I"+name, leftHandDistance);
        }
    }
}
