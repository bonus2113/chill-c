using UnityEngine;
using System.Collections;

public class BigAssDoor : MonoBehaviour {

    public float m_Duration = 2.5f;

    public Transform m_EndPosition;
    private Vector3 m_StartPos;

    private float m_Timer = 0.0f;

    private bool b_Triggered = false;

    private Vector3 m_Direction;

  private Animator anim;

	// Use this for initialization
	void Start () {

    //m_Direction.Normalize();
    anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

        if (b_Triggered)
        {
            m_Timer += Time.deltaTime;


            if (m_Timer >= m_Duration)
            {
                UnityEngine.Camera.main.GetComponent<GameCamera>().StopShake();
                Destroy(this);
            }
        }
	}

    void OnTriggerEnter(Collider col)
    {
      if(col.gameObject == Player.Instance.gameObject && Player.Instance.b_CanOpenDoor)
    {
      b_Triggered = true;
      UnityEngine.Camera.main.GetComponent<GameCamera>().StartShake(10.0f, 0.1f);
      anim.enabled = true;
    }
  }
}
