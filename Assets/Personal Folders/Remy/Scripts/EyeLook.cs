using UnityEngine;
using System.Collections;

public class EyeLook : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 eyeToPlayer = Player.Instance.transform.position - this.transform.position;
        eyeToPlayer.Normalize();
        this.transform.rotation = Quaternion.LookRotation(Vector3.up, eyeToPlayer);

    }
}
