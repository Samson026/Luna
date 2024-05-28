using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject sphere;
    public OVRHand leftHand;
    public OVRHand rightHand;
    List<GameObject> spheres;
    private GameObject attachedSphere;
    // Start is called before the first frame update
    void Start()
    {
        spheres = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnSphere() {
        Vector3 pos = (leftHand.transform.position + rightHand.transform.position) /2;
        GameObject s = Instantiate(sphere, pos, transform.rotation);
        s.GetComponent<SphereController>().rightHand = rightHand;
        s.GetComponent<SphereController>().leftHand = leftHand;
        spheres.Add(s);
        s.GetComponent<SphereController>().AttachSphere();
        attachedSphere = s;     
    }

    public void AttachSphere() {
        GameObject s;
        Transform rightHandPos = this.transform;
        float minDistance = float.MaxValue;

        if (attachedSphere != null) {
            attachedSphere.GetComponent<SphereController>().AttachSphere();
            attachedSphere = null;
            return;
        }

        s = spheres.First();

        foreach (var bone in rightHand.GetComponent<OVRSkeleton>().Bones) {
            if (bone.Id == OVRSkeleton.BoneId.Hand_Thumb1) {
                rightHandPos = bone.Transform;
            }
        }

        foreach (var sphere in spheres) {
            float distance  = Vector3.Distance(sphere.transform.position, rightHandPos.position);
            if (distance < minDistance) {
                minDistance = distance;
                s = sphere;
            }
        }
        s.GetComponent<SphereController>().AttachSphere();
        attachedSphere = s;
    }

    public void DeleteSphere() {
        spheres.Remove(attachedSphere);
        attachedSphere.GetComponent<SphereController>().Remove();
        AttachSphere();
    }
}
