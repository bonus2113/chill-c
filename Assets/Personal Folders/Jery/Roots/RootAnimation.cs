using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootAnimation : ReactOnPlayerNear {

    GameObject player;
    public GameObject target;
    public GameObject voxel1, voxel2;

    public List<Vector3> path = new List<Vector3>();
    Mesh mesh;

    public float size = 0.1f;
    public float distanceRange = 20f;
    public float distanceCutoff = 10f;

    protected override void Start()
    {
        base.Start();
        player = Player.Instance.gameObject;
    }

    List<GameObject> cheatVoxels = new List<GameObject>();
    public void CreateMesh()
    {
        if (path.Count <= 2) return;

        // destroy voxels
        foreach(GameObject v in cheatVoxels)
        {
            DestroyImmediate(v);
        } cheatVoxels = new List<GameObject>();

        //mesh = GetComponent<MeshFilter>().sharedMesh;

        // step path
        List<Vector3> voxels = new List<Vector3>();
        float size_t = size * 1.8f;
        int j = 0;
        for(int i = 1; i < path.Count; i++)
        {
            if(Vector3.Distance(path[i], path[j]) > size)
            {
                Vector3 step = (path[i] - path[j]).normalized * size;

                Vector3 pos = path[j];
                for (int s = 1; Vector3.Distance(pos, path[i]) > size * 1.2f; s++)
                {
                    pos = path[j] + step * s;
                    voxels.Add(new Vector3(Mathf.Round(pos.x / size_t) * size_t, Mathf.Round(pos.y / size_t) * size_t, Mathf.Round(pos.z / size_t) * size_t));
                }
                path[i] = pos;

                j = i;
            }
        }


        for(int i = 0; i < voxels.Count; i++)
        {
            GameObject v = (GameObject)GameObject.Instantiate(Random.Range(0,10) < 7 ? voxel1 : voxel2, voxels[i], Quaternion.identity, transform);
            v.transform.localScale = new Vector3(size, size, size);
            cheatVoxels.Add(v);
        }
    }

    // Update is called once per frame
	public override void UpdateMe() {
        base.UpdateMe();
        Vector3 pos = transform.position;
        pos.y = player.transform.position.y;
        float dist = Vector3.Distance(player.transform.position, pos);
        dist = (dist - distanceCutoff) / distanceRange;
        dist = Mathf.Max(0f, Mathf.Min(1f, dist));
        SetAnimState(dist);
    }

    public void SetAnimState(float val)
    {
        int hideAfter = Mathf.FloorToInt(transform.childCount * val);

        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<MeshRenderer>().enabled = i < hideAfter;
        }
    }
}
