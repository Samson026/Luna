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

    public OVRHand leftHand;
    public OVRHand rightHand;

    private bool grabbed = false;
    public bool attached = false;
    private Vector3 gPosition;
    private List<Vector3> positions;
    public HandGrabInteractable interactables;

    private OVRBone rightBone;
    private OVRBone leftBone;
    private Vector3 home;

    private float maxAudio = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        home = new Vector3(0.00f, 0.00f, 0.00f);
        pdPatch.SendFloat("harm", 3.0f);
        pdPatch.SendFloat("note", 3.0f);
        pdPatch.SendFloat("vol", maxAudio);
        pdPatch.SendFloat("M", 0.5f);
        positions = new List<Vector3>(); 

        StartCoroutine(SetBones());
    }

    // Update is called once per frame
    void Update()
    {
        // get hand pos
        leftHandPos = leftBone.Transform;
        rightHandPos = rightBone.Transform;
        
        // Check if object is grabbed
        
        // if (interactables.Interactors.Count > 0) {
        //     if (!grabbed) {
        //         grabbed = true;
        //     }
        // }
        // else {
        //     if (grabbed) {
        //         attached = !attached;
        //         grabbed = false;
        //         gPosition = transform.position;
        //     }
        // }
       
        if (attached) {
            // send audio data

            pdPatch.SendFloat("ampmod", Gap() * 100);
            pdPatch.SendFloat("note", Distance() * 8);
            pdPatch.SendFloat("I", Angle() * 25);

            // update position
            Vector3 pos = (leftHandPos.position + rightHandPos.position) /2;
            positions.Add(pos);

            // Debug.Log(positions.Count());

            if (positions.Count() >= 20) {
                transform.position = positions[0];
                positions.RemoveAt(0);
            }
        }
    }
    
    private float Gap() {
        return Vector3.Distance(leftHandPos.position, rightHandPos.position);
    }

    private float Angle() {
        float x = rightHandPos.position.x - leftHandPos.position.x;
        float y = rightHandPos.position.y - leftHandPos.position.y; 
        // float z = rightHandPos.position.z - leftHandPos.position.z;

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

    public void AttachSphere() {
        attached = !attached;

        if (!attached)
            StartCoroutine(Ambient());
    }

    IEnumerator SetBones() {
        // wait for bones to init
        OVRSkeleton leftSkeleton = leftHand.GetComponent<OVRSkeleton>();
        OVRSkeleton rightSkeleton = rightHand.GetComponent<OVRSkeleton>();
        while (leftSkeleton.Bones.Count == 0) {
            yield return new WaitForSeconds(1);
        }

        Debug.Log("bone count " + leftSkeleton.Bones.Count);

        // set reference to left and right bones
        foreach (var bone in leftSkeleton.Bones) {
            if (bone.Id == OVRSkeleton.BoneId.Hand_Thumb1) {
                Debug.Log("set bone " + bone);
                leftBone = bone;
            }
        }

        foreach (var bone in rightSkeleton.Bones) {
            if (bone.Id == OVRSkeleton.BoneId.Hand_Thumb1) {
                rightBone = bone;
            }
        }
    }

    IEnumerator Ambient() {
        float vol = maxAudio;
        bool up = false;
        float time;

        time = UnityEngine.Random.Range(0.5f, 0.1f);



        Debug.Log("ambient");

        while (!attached) {
            Debug.Log("ambient here");

            if (up)
                vol = vol + 0.01f;
            else
                vol = vol - 0.01f;

            if (vol >= maxAudio)
                up = false;
            else if (vol <= 0)
                up = true;
            
            pdPatch.SendFloat("vol", vol);

            yield return new WaitForSeconds(time);
        }

        while (vol < maxAudio) {
            vol = vol + 0.01f;
            pdPatch.SendFloat("vol", vol);

            yield return new WaitForSeconds(0.2f);
        }
    }


}
