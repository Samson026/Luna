using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Follow : MonoBehaviour
{
    public GameObject followingMe; // in the inspector drag the gameobject the will be following the player to this field
    public float followDistance;
    public int delay;
    public float speed = 0.1f;
    private List<Vector3> storedPositions;
 
 
    void Awake()
    {
        storedPositions = new List<Vector3>(); //create a blank list
     
        if(!followingMe)
        {
            Debug.Log("The FollowingMe gameobject was not set");
        }      
     
        if(followDistance == 0)
        {
            Debug.Log("Please set distance higher then 0");
        }
    }
 
    void Start ()
    {
 
    }
 
    void Update()
    {
        storedPositions.Add(transform.position);
        // if (Vector3.Distance(transform.position, followingMe.transform.position) > followDistance) {
        if(storedPositions.Count > delay)
            //store the position every frame
        {
            if (Vector3.Distance(transform.position, followingMe.transform.position) > followDistance)
            {
                var step = Time.deltaTime * ((storedPositions[1] - storedPositions[2]) / Time.deltaTime).magnitude;
                followingMe.transform.position = Vector3.MoveTowards(followingMe.transform.position, storedPositions[0], step);// storedPositions[0]; //move the player
            }
            storedPositions.RemoveAt (0); //delete the position that player just move to
        }
    }
}
