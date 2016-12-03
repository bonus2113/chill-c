using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRail : MonoBehaviour {
    public bool b_TrackPlayerOnRail = true;
    [HideInInspector]
    public Vector3[] m_PointsOnRail;

    void Awake() {
        RailPoint[] temp = this.GetComponentsInChildren<RailPoint>();
        m_PointsOnRail = new Vector3[temp.Length];
        for (int i = 0; i < temp.Length; i++) {
            m_PointsOnRail[i] = temp[i].transform.position;
        }
        temp[0].b_Trigger = true;
        temp[0].SetParentRail(this);
        temp[0].b_Track = b_TrackPlayerOnRail;
        temp[temp.Length - 1].b_Trigger = true;
        temp[temp.Length - 1].SetParentRail(this);
        temp[temp.Length - 1].b_Track = b_TrackPlayerOnRail;
    }
}