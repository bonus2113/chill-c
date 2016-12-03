using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class FountainSimulate : MonoBehaviour {

  struct GPUParticle
  {
    public Vector3 pos;
    public float _pad1;
    public Color col;
  }

  public ComputeShader SimulateShader;
  public int Resolution;
  public ParticleSystem particlesToRender;
  public int FrameRate = 10;
  public Shader ParticleShader;
  public float brightness = 1;

  Material particleMaterial;
  int generatePointsKernel;

  ParticleSystem.Particle[] particles;

  ComputeBuffer drawIndirectCmdBuffer;
  ComputeBuffer pointBuffer;

  ComputeBuffer vertexBuffer;

  float frameTime;

  Dictionary<UnityEngine.Camera, CommandBuffer> cameras = new Dictionary<UnityEngine.Camera, CommandBuffer>();

  // Use this for initialization
  void Awake ()
  {
    particleMaterial = new Material(ParticleShader);

    generatePointsKernel = SimulateShader.FindKernel("GeneratePoints");

    particles = new ParticleSystem.Particle[particlesToRender.maxParticles];

    frameTime = 1.0f / FrameRate;

    pointBuffer = new ComputeBuffer(Resolution * Resolution * Resolution, 12 * sizeof(float), ComputeBufferType.Append);
    pointBuffer.SetCounterValue(0);

    drawIndirectCmdBuffer = new ComputeBuffer(1, 16, ComputeBufferType.IndirectArguments);

    drawIndirectCmdBuffer.SetData(new int[4] { 36, 1, 0, 0 });

    vertexBuffer = new ComputeBuffer(36, 12);
    vertexBuffer.SetData(new Vector3[]
    {
        new Vector3(-1.0f,-1.0f,-1.0f),
        new Vector3(-1.0f,-1.0f, 1.0f),
        new Vector3(-1.0f, 1.0f, 1.0f),
        new Vector3( 1.0f, 1.0f,-1.0f),
        new Vector3(-1.0f,-1.0f,-1.0f),
        new Vector3(-1.0f, 1.0f,-1.0f),
        new Vector3( 1.0f,-1.0f, 1.0f),
        new Vector3(-1.0f,-1.0f,-1.0f),
        new Vector3( 1.0f,-1.0f,-1.0f),
        new Vector3( 1.0f, 1.0f,-1.0f),
        new Vector3( 1.0f,-1.0f,-1.0f),
        new Vector3(-1.0f,-1.0f,-1.0f),
        new Vector3(-1.0f,-1.0f,-1.0f),
        new Vector3(-1.0f, 1.0f, 1.0f),
        new Vector3(-1.0f, 1.0f,-1.0f),
        new Vector3( 1.0f,-1.0f, 1.0f),
        new Vector3(-1.0f,-1.0f, 1.0f),
        new Vector3(-1.0f,-1.0f,-1.0f),
        new Vector3(-1.0f, 1.0f, 1.0f),
        new Vector3(-1.0f,-1.0f, 1.0f),
        new Vector3( 1.0f,-1.0f, 1.0f),
        new Vector3( 1.0f, 1.0f, 1.0f),
        new Vector3( 1.0f,-1.0f,-1.0f),
        new Vector3( 1.0f, 1.0f,-1.0f),
        new Vector3( 1.0f,-1.0f,-1.0f),
        new Vector3( 1.0f, 1.0f, 1.0f),
        new Vector3( 1.0f,-1.0f, 1.0f),
        new Vector3( 1.0f, 1.0f, 1.0f),
        new Vector3( 1.0f, 1.0f,-1.0f),
        new Vector3(-1.0f, 1.0f,-1.0f),
        new Vector3( 1.0f, 1.0f, 1.0f),
        new Vector3(-1.0f, 1.0f,-1.0f),
        new Vector3(-1.0f, 1.0f, 1.0f),
        new Vector3( 1.0f, 1.0f, 1.0f),
        new Vector3(-1.0f, 1.0f, 1.0f),
        new Vector3( 1.0f,-1.0f, 1.0f)
    });

    particleMaterial.SetBuffer("Vertices", vertexBuffer);
  }

  public void OnDisable()
  {
    foreach (var cam in cameras)
    {
      if (cam.Key)
      {
        cam.Key.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, cam.Value);
      }
    }
  }

  void OnWillRenderObject()
  {
    var act = gameObject.activeInHierarchy && enabled;
    if (!act)
    {
      OnDisable();
      return;
    }

    var cam = UnityEngine.Camera.main;
    if (!cam)
      return;

    CommandBuffer buf = null;
    if (cameras.ContainsKey(cam))
    {
      buf = cameras[cam];
      buf.Clear();
    }
    else
    {
      buf = new CommandBuffer();
      buf.name = "VoxelParticles";
      cameras[cam] = buf;

      cam.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, buf);
    }

    ComputeBuffer.CopyCount(pointBuffer, drawIndirectCmdBuffer, 4);
    particleMaterial.SetBuffer("VertexBuffer", vertexBuffer);
    particleMaterial.SetBuffer("PointBuffer", pointBuffer);
    particleMaterial.SetInt("Resolution", Resolution);
    buf.DrawProceduralIndirect(transform.localToWorldMatrix, particleMaterial, -1, MeshTopology.Triangles, drawIndirectCmdBuffer);

  }


  // Update is called once per frame
  void Update()
  {
    particleMaterial.SetFloat("Brightness", brightness);

    frameTime -= Time.deltaTime;
    if (frameTime > 0) return;

    frameTime = 1.0f / FrameRate;

    int count = particlesToRender.GetParticles(particles);

    if (count != 0)
    {
      ComputeBuffer particleBuffer = new ComputeBuffer(count, 8 * sizeof(float));

      GPUParticle[] gpuParticles = new GPUParticle[count];


      for (int i = 0; i < count; i++)
      {
        gpuParticles[i].pos = particles[i].position;
        gpuParticles[i].col = particles[i].GetCurrentColor(particlesToRender);
      }

      particleBuffer.SetData(gpuParticles);
      SimulateShader.SetBuffer(generatePointsKernel, "Particles", particleBuffer);

      var mat = transform.localToWorldMatrix.transpose;
      SimulateShader.SetFloats("Object2World", new float[] {
        mat.m00, mat.m01, mat.m02, mat.m03,
        mat.m10, mat.m11, mat.m12, mat.m13,
        mat.m20, mat.m21, mat.m22, mat.m23,
        mat.m30, mat.m31, mat.m32, mat.m33,
      });

      SimulateShader.SetInt("Resolution", Resolution);


      pointBuffer.SetCounterValue(0);
      SimulateShader.SetBuffer(generatePointsKernel, "PointBufferOutput", pointBuffer);

      SimulateShader.Dispatch(generatePointsKernel, Resolution / 8, Resolution / 8, Resolution / 8);

      particleBuffer.Release();
      OnWillRenderObject();

    }
  }

  void OnDestroy()
  {
    pointBuffer.Release();
    drawIndirectCmdBuffer.Release();
    vertexBuffer.Release();
  }

  void OnDrawGizmosSelected()
  {
    DrawGizmo(true);
  }

  void OnDrawGizmos()
  {
    DrawGizmo(false);
  }

  void DrawGizmo(bool selected)
  {
    var col = new Color(0.0f, 0.7f, 1f, 1.0f);
    col.a = selected ? 0.5f : 0.3f;
    Gizmos.color = col;
    Gizmos.matrix = transform.localToWorldMatrix;
    Gizmos.DrawCube(Vector3.zero, Vector3.one);
    col.a = selected ? 0.8f : 0.6f;
    Gizmos.color = col;
    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
  }

}
