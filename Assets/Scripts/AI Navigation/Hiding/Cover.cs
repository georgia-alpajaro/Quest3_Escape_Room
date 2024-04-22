using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Just a class to display where we can put the area for the enemy to hide behind
public class Cover : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.3f);
    }
}
