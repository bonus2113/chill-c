using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarm : MonoBehaviour {

  public bool goToTarget;
  public float targetStrength = 0;
  public Transform targetSpot;

  public float windStrength;
  [SerializeField]
  private Vector3 wind;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public Vector3 Wind()
  {
    return wind * windStrength;
  }

  public Vector3 SteeringVelocity(Vector3 pos)
  {
    if (!goToTarget || !targetSpot) return Vector3.zero;
    return Vector3.ClampMagnitude(targetSpot.position - pos, 1.0f) * targetStrength;
  }

  public void OnDrawGizmos()
  {
    if(targetSpot)
    {
      Gizmos.DrawSphere(targetSpot.position, 0.25f);
    }
  }
}
