using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distance : MonoBehaviour
{
    public LibPdInstance pdPatch;
    public Transform targetLocation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (Vector3.Distance(targetLocation.position, transform.position) * 1000);
        pdPatch.SendFloat("distance", distance);
    }
}
