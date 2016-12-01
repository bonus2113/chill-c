using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalAnimation : MonoBehaviour {

    public static int[] map = {1,3, 3,2, 3,0, 2,0, 1,1, 1,0, 3,1, 2,2, 0,1, 0,3, 2,1, 1,2, 0,2, 3,3, 0,0, 2,3};

    public GameObject animationPrefab;
    List<GameObject> parts = new List<GameObject>();
    List<float> zspeeds = new List<float>();

    Material mat;
    Texture2D tex;
    int size;

    float time = 0f;
    int index = 0;
    int visi = 0;

    void Start()
    {   
        // get texture
        mat = GetComponent<MeshRenderer>().material;
        tex = mat.mainTexture as Texture2D;
        size = mat.mainTexture.width / 4;
    }

    void Update()
    {
        for (int i = parts.Count - 1; i >= 0; i--)
        {
            // calc position and decrease speed
            GameObject obj = parts[i];
            Vector3 pos = obj.transform.localPosition;
            pos.z += Time.deltaTime * zspeeds[i];
            zspeeds[i] *= 0.99f;
            zspeeds[i] = zspeeds[i] > 0.2f ? zspeeds[i] : 0.2f;
            // update position
            obj.transform.localPosition = pos;

            // check if arrived
            if (pos.z > -0.435f)
            {
                parts.RemoveAt(i);
                zspeeds.RemoveAt(i);
                Destroy(obj);
                visi++;
                GetComponent<MeshRenderer>().material.SetFloat("_Cutoff", 0.18f + 0.05f * visi);
            }
        }
    }

    public Vector3 GetStartPosition(int i)
    {
        // clone part of texture
        int x = (int)map[i * 2];
        int y = (int)map[i * 2 + 1];
        Vector3 s = transform.localScale;
        return new Vector3(s.x * -0.3725f + x * 0.25f * s.x, s.y * -0.3725f + y * 0.25f * s.y, -1.0f * s.z);
    }

    public void AnimateRunepart(int i)
    {
        // clone part of texture
        int x = (int)map[i*2];
        int y = (int)map[i*2+1];
        Texture2D part = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] cols = tex.GetPixels(x * size, y * size, size, size);
        part.SetPixels(0, 0, size, size, cols);
        part.Apply();

        // create mesh
        GameObject obj = (GameObject)GameObject.Instantiate(animationPrefab);
        obj.transform.parent = transform;
        obj.transform.localPosition = GetStartPosition(i);
        obj.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", part as Texture);
        parts.Add(obj);
        zspeeds.Add(1.2f);
    }
}
