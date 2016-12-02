using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FixShaders : MonoBehaviour
{

  [MenuItem("Grove/FixBullshit/FixShaders")]
  static void FixShadersFun() //create orphans lol
  {
    var guids = Selection.assetGUIDs;

    var shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/Shaders/UnlitTexture.shader");
    foreach(var guid in guids)
    {
      var assetPath = AssetDatabase.GUIDToAssetPath(guid);
      var asset = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

      if (!asset) continue;

      if(asset.shader.name == "Unlit/Texture")
      {
        asset.shader = shader;
      }
    }


    AssetDatabase.Refresh();
    AssetDatabase.SaveAssets();
  }

}
