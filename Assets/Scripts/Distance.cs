using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distance : MonoBehaviour
{
    public LibPdInstance pdPatch;
    public Transform leftHand;
    public Transform rightHand;
    // Start is called before the first frame update
    void Start()
    {
        pdPatch.SendFloat("harm", 9.0f);
        pdPatch.SendFloat("note", 7.0f);
        pdPatch.SendFloat("detune", 0.0f);
        pdPatch.SendFloat("vol", 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
       
        // pdPatch.SendFloat("ampmod", 1.0f);
        float leftHandDistance = Vector3.Distance(leftHand.position, transform.position);
        leftHandDistance = leftHandDistance * 16;
        float rightHandDistance = Vector3.Distance(rightHand.position, transform.position);
        rightHandDistance = rightHandDistance * 100;
        Debug.Log(rightHandDistance);
        Debug.Log(leftHandDistance);
        pdPatch.SendFloat("ampmod", rightHandDistance);
        pdPatch.SendFloat("harm", leftHandDistance);
    }
}
