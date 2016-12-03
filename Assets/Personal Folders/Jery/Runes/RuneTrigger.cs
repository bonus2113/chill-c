using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneTrigger : MonoBehaviour {

    public bool triggered;
    public RuneTrigger triggers;


    float time = 0f;
	void Update () {
        time += Time.deltaTime;
        float t = 1f + Mathf.Abs(Mathf.Sin(time)) * 0.7f;
        GetComponent<MeshRenderer>().material.SetFloat("_Emission", t);
	}
}
