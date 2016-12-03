using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailPoint : MonoBehaviour {
    [HideInInspector]
    public bool b_Trigger = false;
    [HideInInspector]
    public bool b_Track = false;

    private CameraRail m_ParentRail = null;

    void Start() {
        if(b_Trigger) {
            BoxCollider bc = this.gameObject.AddComponent<BoxCollider>();
            bc.isTrigger = true;
        }
    }

    void OnTriggerEnter (Collider other) {
        if (other.tag != "MainCamera") {
            return;
        }
        if (m_ParentRail != null) {
            if (CamControl.Instance.m_CamState != CamControl.CamState.FollowRail) {
                CamControl.Instance.SetCamRail(m_ParentRail, b_Track);
            } else {
                CamControl.Instance.SetCamTopDown(Vector3.zero);
            }
        }
    }

    public void SetParentRail(CameraRail r) {
        m_ParentRail = r;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);
    }
}