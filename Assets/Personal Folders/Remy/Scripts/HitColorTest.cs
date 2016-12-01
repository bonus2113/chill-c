using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitColorTest : MonoBehaviour {

    private Material m_Mat = null;

    [SerializeField]
    private Texture2D m_Texture = null;

    void Awake()
    {
        m_Mat = this.GetComponent<Renderer>().material;
    }

	// Update is called once per frame
	void Update () {

        RaycastHit hitInfo;
        if (Physics.Raycast(this.transform.position, Vector3.down, out hitInfo, 5.0f, (1 << LayerMask.NameToLayer("GroundRaycast"))))
        {
            m_Mat.color = m_Texture.GetPixelBilinear(hitInfo.textureCoord.x, hitInfo.textureCoord.y);
        }
        else
        {
            m_Mat.color = Color.red;
        }
	}
}
