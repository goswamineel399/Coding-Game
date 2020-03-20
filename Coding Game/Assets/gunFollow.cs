using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunFollow : MonoBehaviour
{
    public Transform toFollow;
    public Vector3 offset;
    public bool followRotation;

    // Update is called once per frame
    void Update()
    {
        transform.position = toFollow.position + offset;
        if (followRotation)
        {
            transform.rotation = toFollow.rotation;
        }
    }
}
