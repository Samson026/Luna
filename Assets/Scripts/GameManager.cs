using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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
        attachedSphere = new GameObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnSphere() {
        GameObject s = Instantiate(sphere);
        s.GetComponent<SphereController>().rightHand = rightHand;
        s.GetComponent<SphereController>().leftHand = leftHand;
        s.GetComponent<SphereController>().startPosition = s.transform.position;
        // Debug.Log("running Creating " + s);
        spheres.Add(s);
        // Debug.Log("running Creating " + spheres.First());       
    }

    public void AttachSphere() {
        // Debug.Log("running attachSphere");
        Transform rightHandPos = this.transform;
        float minDistance = float.MaxValue;
        GameObject s = spheres.First();

        // Debug.Log("running len" + spheres.Count);

        foreach (var bone in rightHand.GetComponent<OVRSkeleton>().Bones) {
            if (bone.Id == OVRSkeleton.BoneId.Hand_Thumb1) {
                rightHandPos = bone.Transform;
            }
        }

        foreach (var sphere in spheres) {
            float distance  = Vector3.Distance(sphere.transform.position, rightHandPos.position);
            // Debug.Log("running dis " + distance + " " + minDistance );
            if (distance < minDistance) {
                // Debug.Log("running settings a sphere");
                minDistance = distance;
                s = sphere;
            }
        }
        // Debug.Log("running attach2 + " + s);
        s.GetComponent<SphereController>().AttachSphere();
        attachedSphere = s;
    }

    public void DeleteSphere() {
        Debug.Log("oldmate + " + attachedSphere);

        spheres.Remove(attachedSphere);
        Destroy(attachedSphere);
        attachedSphere = new GameObject();
    }
}
