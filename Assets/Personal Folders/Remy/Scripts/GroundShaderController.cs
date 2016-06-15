using UnityEngine;
using System.Collections;

public class GroundShaderController : MonoBehaviour {

    private Material m_Material = null;
    private Material m_BlitMaterial = null;
    private Material m_FadeMaterial = null;

    private RenderTexture m_RenderTex = null;
    private RenderTexture m_RenderTexNoFade = null;

    public Texture2D m_RadialGraphic = null;
    private const float c_RadialUVSize = 0.01f;
    private const int c_TexSize = 512;

    private float m_DebugTimer = 0.0f;
    private const float c_DebugInterval = 1.0f;

    private float m_HalfSize = 5.0f;
    private float m_BlobSize = 0.5f;
    private Vector2 m_UVOffset = Vector2.zero;

    public RenderTexture GetRenderTexture
    {
        get
        {
            return m_RenderTex;
        }
    }

    void Awake()
    {
        m_HalfSize *= this.transform.localScale.x;
        m_BlobSize /= this.transform.localScale.x;

        m_UVOffset.x += this.transform.position.x / (m_HalfSize * 2.0f);
        m_UVOffset.y += this.transform.position.z / (m_HalfSize * 2.0f);

        this.m_Material = this.GetComponent<Renderer>().material;
        m_RenderTex = new RenderTexture(c_TexSize, c_TexSize, 0, RenderTextureFormat.ARGB32);
        m_RenderTex.useMipMap = false;
        m_RenderTex.generateMips = false;

        m_RenderTexNoFade = new RenderTexture(c_TexSize, c_TexSize, 0, RenderTextureFormat.ARGB32);
        m_RenderTexNoFade.useMipMap = false;
        m_RenderTexNoFade.generateMips = false;

        m_RenderTex.Create();
        m_RenderTexNoFade.Create();

        m_Material.SetTexture("_GardenMap", m_RenderTex);
        m_Material.SetTexture("_GardenMap2", m_RenderTexNoFade);

        m_BlitMaterial = new Material(Shader.Find("Unlit/blitShader"));
        m_BlitMaterial.SetInt("_TexSize", c_TexSize);

        m_FadeMaterial = new Material(Shader.Find("Unlit/tendToBlack"));
        //m_FadeMaterial.SetTexture("_MainTex", m_RenderTex);

    }

    public void BlitToUVPosition(Vector2 uv, float radiusUV, bool fade)
    {
        int radiusPixel = (int)(radiusUV * c_TexSize);
        m_BlitMaterial.SetVector("_PosSize", new Vector4((uv.x) * c_TexSize - radiusPixel/2, (uv.y) * c_TexSize - radiusPixel/2, radiusPixel, radiusPixel));

        if (fade)
        {
            Graphics.Blit(m_RadialGraphic, m_RenderTex, m_BlitMaterial);
        }
        else
        {
            Graphics.Blit(m_RadialGraphic, m_RenderTexNoFade, m_BlitMaterial);
        }
    }

    // Use this for initialization
    void Start () {

        GroundShadingManager.RegisterSelf(this);
    }

    void Update()
    {
        Graphics.Blit(null, m_RenderTex, m_FadeMaterial);
    }
	
	// Update is called once per frame
	public void BlitAtPosition (Vector2 pos, float scale = 1.0f, bool fade = true) {

        Vector2 uvPos = pos / (m_HalfSize * 2.0f);
        uvPos += new Vector2(0.5f, 0.5f);

        BlitToUVPosition(uvPos + m_UVOffset, m_BlobSize * scale, fade);
    }

    void OnRenderObject()
    {
       
    }
}
