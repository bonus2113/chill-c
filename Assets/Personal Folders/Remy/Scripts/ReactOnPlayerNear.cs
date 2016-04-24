using UnityEngine;
using System.Collections;

public class ReactOnPlayerNear : MonoBehaviour {

    public float m_ActivationDistance = 10.0f;

    private float m_CurrentDistance = 0.0f;

    protected bool b_PlayerNear = false;

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    protected virtual void Update() {

        Vector3 myPos = this.transform.position;
        myPos.y = 0.0f;
        m_CurrentDistance = (Player.Instance.transform.position - myPos).magnitude;

        if (!b_PlayerNear)
        {
            if (m_CurrentDistance <= m_ActivationDistance)
            {
                OnPlayerNear();
            }
        }
        else
        {
            if (m_CurrentDistance >= m_ActivationDistance)
            {
                OnPlayerFar();
            }
        }
	
	}

    protected virtual void OnPlayerNear()
    {
        b_PlayerNear = true;
        //Debug.Log("PlayerNear.");
    }

    protected virtual void OnPlayerFar()
    {
        b_PlayerNear = false;
        //Debug.Log("PlayerFar.");
    }
}
