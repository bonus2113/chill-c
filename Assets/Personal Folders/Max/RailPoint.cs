using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailPoint : MonoBehaviour {
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);
    }
}