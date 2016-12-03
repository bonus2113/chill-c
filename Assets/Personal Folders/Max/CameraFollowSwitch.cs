using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowSwitch : MonoBehaviour {
    public Vector3 m_FollowOffset = new Vector3(0, 3, 0);
    void OnTriggerEnter(Collider other) {
        if(other.transform != Player.Instance.transform) {
            return;
        }
        if(CamControl.Instance.m_CamState != CamControl.CamState.Follow) {
            CamControl.Instance.SetCamFollow(m_FollowOffset);
            Player.Instance.b_CamRelativeMovement = true;
        } else {
            CamControl.Instance.SetCamTopDown(Vector3.zero);
            Player.Instance.b_CamRelativeMovement = false;
        }
    }
}
