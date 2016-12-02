using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRail : MonoBehaviour {
    [HideInInspector]
    public Vector3[] m_PointsOnRail;

    void Awake() {
        RailPoint[] temp = this.GetComponentsInChildren<RailPoint>();
        m_PointsOnRail = new Vector3[temp.Length];
        for (int i = 0; i < temp.Length; i++) {
            m_PointsOnRail[i] = temp[i].transform.position;
        }
    }
}