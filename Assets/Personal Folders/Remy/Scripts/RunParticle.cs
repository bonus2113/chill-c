using UnityEngine;
using System.Collections;

public class RunParticle : MonoBehaviour {


    private Vector2 m_MinMaxLifeTime = new Vector2(0.3f, 0.5f);
    private Vector2 m_MinMaxScale = new Vector2(0.5f, 1.0f);

    private float m_LifeTime = 0.0f;
    private float m_Timer = 0.0f;

    private Vector3 m_Direction = Vector3.zero;
    private Vector2 m_MinMaxPlayerVelFactor = new Vector2(0.05f, 0.2f);
    private float m_MaxRightDeviation = 90.0f;
    private float m_MaxUpVelocity = 2.0f;

    private float m_StartScale = 1.0f;

    public AnimationCurve m_ScaleCurve;

    void OnEnable()
    {
        m_LifeTime = Random.Range(m_MinMaxLifeTime.x, m_MinMaxLifeTime.y);
        m_Timer = 0.0f;

        this.transform.position = Player.Instance.transform.position;

        m_StartScale = Random.Range(m_MinMaxScale.x, m_MinMaxScale.y);
        this.transform.localScale = new Vector3(0, 0, 0);

        this.transform.rotation = Random.rotationUniform;
    }

    void OnDisable()
    {

    }

    public void Activate(Vector3 playerVel)
    {
        this.gameObject.SetActive(true);
        Vector3 dir = -playerVel * Random.Range(m_MinMaxPlayerVelFactor.x, m_MinMaxPlayerVelFactor.y);

        dir = Quaternion.Euler(Vector3.up * Random.Range(-m_MaxRightDeviation, m_MaxRightDeviation)) * dir;
        dir.y += Random.Range(0.0f, m_MaxUpVelocity);

        this.m_Direction = dir;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        m_Timer += Time.deltaTime;

        if (m_Timer >= m_LifeTime)
        {
            this.gameObject.SetActive(false);
        }

        this.transform.position += m_Direction * Time.deltaTime;

        float scale = m_StartScale * m_ScaleCurve.Evaluate(m_Timer / m_LifeTime);

        this.transform.localScale = new Vector3(scale, scale, scale);
    }
}
