using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidAttractor : MonoBehaviour {
  public float strength = 1.0f;

	// Use this for initialization
	void Start () {
    gameObject.layer = LayerMask.NameToLayer("BoidTractor");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
