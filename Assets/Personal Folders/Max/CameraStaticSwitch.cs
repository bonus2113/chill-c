using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStaticSwitch : MonoBehaviour {
    private Transform m_CamTrans;
    public float m_CamShotDrift = 0.1f;
    void Start() {
        Transform[] t = GetComponentsInChildren<Transform>();
        for (int i = 0; i< t.Length; i++) {
            if(t[i] != this.transform) {
                m_CamTrans = t[i];
                i = t.Length + 1;
            }
        }
    }
    void OnTriggerEnter(Collider other) {
        if(other.transform != Player.Instance.transform) {
            return;
        }
        CamControl.Instance.SetCamStaticShot(m_CamTrans, m_CamShotDrift);
    }
    void OnTriggerExit(Collider other) {
        if (other.transform != Player.Instance.transform) {
            return;
        }
        CamControl.Instance.SetCamTopDown(Vector3.zero);
    }
}