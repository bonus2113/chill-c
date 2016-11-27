using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UnparentEverything : MonoBehaviour {

    [MenuItem("Grove/FixBullshit/UnparentAll")]
    static void UnparentAllChildren() //create orphans lol
    {
        var selectionTransform = Selection.activeTransform;

        UnparentChildren(selectionTransform);
    }

    static void UnparentChildren(Transform parent)
    {
        while (parent.childCount != 0)
        {
            Transform child = parent.GetChild(0);
            child.SetParent(null); //unparent child
            //unparent child's children
            UnparentChildren(child);
        }
    }
}
