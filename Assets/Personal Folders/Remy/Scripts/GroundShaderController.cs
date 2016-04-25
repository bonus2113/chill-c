using UnityEngine;
using System.Collections;

public class GroundShaderController : MonoBehaviour {

    private Material m_Material = null;
    private Material m_BlitMaterial = null;

    private RenderTexture m_RenderTex = null;

    public Texture2D m_RadialGraphic = null;
    private const float c_RadialUVSize = 0.01f;
    private const int c_TexSize = 512;

    private float m_DebugTimer = 0.0f;
    private const float c_DebugInterval = 1.0f;

    void Awake()
    {
        this.m_Material = this.GetComponent<Renderer>().material;
        m_RenderTex = new RenderTexture(c_TexSize, c_TexSize, 0, RenderTextureFormat.ARGB32);
        m_RenderTex.useMipMap = true;
        m_RenderTex.generateMips = false;

        m_RenderTex.Create();

        m_Material.SetTexture("_GardenMap", m_RenderTex);

        m_BlitMaterial = new Material(Shader.Find("Unlit/blitShader"));
        m_BlitMaterial.SetInt("_TexSize", c_TexSize);

        BlitToUVPosition(new Vector2(0.5f, 0.5f), 0.5f);
    }

    private void BlitToUVPosition(Vector2 uv, float radiusUV)
    {
        int radiusPixel = (int)(radiusUV * c_TexSize);
        m_BlitMaterial.SetVector("_PosSize", new Vector4((uv.x) * c_TexSize - radiusPixel/2, (uv.y) * c_TexSize - radiusPixel/2, radiusPixel, radiusPixel));


        Graphics.Blit(m_RadialGraphic, m_RenderTex, m_BlitMaterial);
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        m_DebugTimer += Time.deltaTime;

        if (m_DebugTimer >= c_DebugInterval)
        {
            m_DebugTimer -= c_DebugInterval;
            BlitToUVPosition(new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)), Random.Range(0.1f, 0.5f));
        }
	}
}
