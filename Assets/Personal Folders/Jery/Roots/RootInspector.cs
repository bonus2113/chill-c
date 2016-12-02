using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RootAnimation))]
public class RootInspector : Editor
{

    public override void OnInspectorGUI()
    {
        RootAnimation roots = (RootAnimation)target;
        DrawDefaultInspector();

        GUILayout.Box("Left Ctrl + Left Mouse Button - Set path point", GUILayout.ExpandWidth(true));
    }

    void OnSceneGUI()
    {
        RootAnimation roots = (RootAnimation)target;

        if (Event.current.control)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        if (Event.current.control && Event.current.type == EventType.MouseDown)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 hitpos = Intersect.RaycastMesh(roots.target.GetComponent<MeshFilter>().sharedMesh, roots.target.transform.position, ray);

            if(hitpos != Vector3.zero)
            {
                //AddPointToPath(0, hitpos - ray.direction * 0.7f);
            }

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, 50))
            {
                AddPointToPath(0, hit.point);
            }

        }
    }



    void AddPointToPath(int path, Vector3 point)
    {
        RootAnimation roots = (RootAnimation)target;
        roots.path.Add(point);
        roots.CreateMesh();
    }
}