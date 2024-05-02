using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction.HandGrab;
using Unity.VisualScripting;
using UnityEngine;

public class SphereController : MonoBehaviour
{
    public LibPdInstance pdPatch;
    private Transform leftHandPos;
    private Transform rightHandPos;

    public OVRHand leftHand;
    public OVRHand rightHand;

    public bool attached = false;
    public Vector3 startPosition;
    private List<Vector3> positions;
    public HandGrabInteractable interactables;

    private OVRBone rightBone;
    private OVRBone leftBone;

    private float maxAudio = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        pdPatch.SendFloat("harm", 10.0f);
        pdPatch.SendFloat("note", 3.0f);
        pdPatch.SendFloat("vol", maxAudio);
        pdPatch.SendFloat("M", 0.3f);
        positions = new List<Vector3>(); 

        StartCoroutine(SetBones());
        startPosition = new Vector3(0, 1.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // get hand pos
        leftHandPos = leftBone.Transform;
        rightHandPos = rightBone.Transform;
        
        // Check if object is grabbed
       
        if (attached) {
            // send audio data
            UpdateIndex();
            UpdateNote();
            UpdateHarm();
            UpdateMod();
            UpdateAmpMod();
            UpdateSize();

            // update position
            Vector3 pos = (leftHandPos.position + rightHandPos.position) /2;
            positions.Add(pos);

            if (positions.Count() >= 20) {
                transform.position = positions[0];
                positions.RemoveAt(0);
            }
        }
    }
    
    private float Gap() {
        return Vector3.Distance(leftHandPos.position, rightHandPos.position) / 1f;
    }

    private void UpdateNote() {
        float note;
        float steps = 8f;

        note = Angle() * steps;
        pdPatch.SendFloat("note", note); 
    }

    private void UpdateHarm() {
        float harm;
        float steps = 13f;

        harm = Gap() * steps;
        if (harm < 1)
            harm = 1;
        pdPatch.SendFloat("harm", harm); 
    }

    private void UpdateIndex() {
        float index;
        float steps = 5f;

        index = DistanceX() * steps;
        Debug.Log("index " + index);
        pdPatch.SendFloat("I", index); 
    }

    private void UpdateAmpMod() {
        float index;
        float steps = 5f;

        index = DistanceY() * steps;
        Debug.Log("index " + index);
        pdPatch.SendFloat("amp", index); 
    }

    private void UpdateMod() {
        float mod;
        float steps = 1f;

        mod = DistanceZ() * steps;
        pdPatch.SendFloat("M", mod);  
    }

    private void UpdateSize() {
        float scale = (Gap() * 0.4f);
        Vector3 scaleVec = new Vector3(scale, scale, scale);


        transform.GetChild(1).localScale = scaleVec;
    }

    private float Angle() {
        float x = rightHandPos.position.x - leftHandPos.position.x;
        float y = rightHandPos.position.y - leftHandPos.position.y; 

        float m = y/x;
        float angleRadians = Mathf.Atan(m);
        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        if (rightHandPos.position.y > leftHandPos.position.y) {
            angleDegrees = MathF.Abs(angleDegrees) + 90f;
        }
        else {
            if (angleDegrees > 0)
                angleDegrees =  90f + angleDegrees;
            else
                angleDegrees =  angleDegrees + 90f;
        }

        return angleDegrees / 180;
    }

    private float DistanceX() {
        Vector3 mid = (rightHandPos.position + leftHandPos.position) /2;
        float d = Mathf.Abs(mid.x - startPosition.x);
        return d / 2f;
    }
    private float DistanceY() {
        Vector3 mid = (rightHandPos.position + leftHandPos.position) /2;
        float d = Mathf.Abs(mid.y - startPosition.y);
        return d / 2f;
    }

    private float DistanceZ() {
        Vector3 mid = (rightHandPos.position + leftHandPos.position) /2;
        float d = Mathf.Abs(mid.z - startPosition.z);
        return d / 2f;
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

        // set reference to left and right bones
        foreach (var bone in leftSkeleton.Bones) {
            if (bone.Id == OVRSkeleton.BoneId.Hand_Thumb1) {
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

        time = Gap() * 0.5f;// UnityEngine.Random.Range(0.5f, 0.1f);

        while (!attached) {

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
