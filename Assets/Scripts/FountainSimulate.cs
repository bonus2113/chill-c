using UnityEngine;
using System.Collections;

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

  RenderTexture densityTex;
  int densityTransferKernel;
  MeshRenderer meshRend;

  ParticleSystem.Particle[] particles;

  float frameTime;

  // Use this for initialization
  void Awake ()
  {
    meshRend = GetComponent<MeshRenderer>();
    densityTex = new RenderTexture(Resolution, Resolution, 0, RenderTextureFormat.ARGBFloat);
    densityTex.isVolume = true;
    densityTex.enableRandomWrite = true;
    densityTex.useMipMap = false;
    densityTex.filterMode = FilterMode.Point;
    densityTex.volumeDepth = Resolution;
    densityTex.Create();

    densityTransferKernel = SimulateShader.FindKernel("DensityTransfer");
    SimulateShader.SetTexture(densityTransferKernel, "Density", densityTex);
    meshRend.material.SetVector("_VolumeDimensions", new Vector4(Resolution, Resolution, Resolution));
    meshRend.material.SetTexture("_DensityTex", densityTex);

    particles = new ParticleSystem.Particle[particlesToRender.maxParticles];

    frameTime = 1.0f / FrameRate;
  }



  // Update is called once per frame
  void Update ()
  {
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
        gpuParticles[i].col.a = 1;

        //gpuParticles[i].pos += particlesToRender.transform.position;
      }

      particleBuffer.SetData(gpuParticles);
      SimulateShader.SetBuffer(densityTransferKernel, "Particles", particleBuffer);

      var mat = transform.localToWorldMatrix.transpose;
      SimulateShader.SetFloats("Object2World", new float[] {
        mat.m00, mat.m01, mat.m02, mat.m03,
        mat.m10, mat.m11, mat.m12, mat.m13,
        mat.m20, mat.m21, mat.m22, mat.m23,
        mat.m30, mat.m31, mat.m32, mat.m33,
      });

      SimulateShader.SetInt("Resolution", Resolution);
      SimulateShader.SetFloat("Time", Time.timeSinceLevelLoad);
      SimulateShader.SetFloat("dT", Time.deltaTime);

      SimulateShader.Dispatch(densityTransferKernel, Resolution / 8, Resolution / 8, Resolution / 8);

      meshRend.material.SetTexture("_DensityTex", densityTex);
      particleBuffer.Release();
    }
  }
}
