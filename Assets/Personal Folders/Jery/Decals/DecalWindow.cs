using UnityEngine;
using UnityEditor;
using System.IO;
public class DecalWindow : EditorWindow
{


    // Add menu named "My Window" to the Window menu
    [MenuItem("Grove/Decals")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DecalWindow window = (DecalWindow)EditorWindow.GetWindow(typeof(DecalWindow));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Create Decal") && Selection.activeObject is Texture2D)
        {
            byte[] bytes = createAlphaMapFromTexture((Texture2D)Selection.activeObject);

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            path = Application.dataPath + path.Substring(6, path.LastIndexOf('.') - 6) + "_alpha.png";

            File.WriteAllBytes(path, bytes);
            Debug.Log("Saved as: " + path);
        }
    }


    byte[] createAlphaMapFromTexture(Texture2D tex)
    {
        Texture2D map = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        
        int size = tex.width / 8;
        Color[] cols = new Color[size*size];
        for (int i = 0; i < size * size; i++ )
        {
            cols[i] = new Color(1f, 1f, 1f, Random.Range(0, 100) * 0.01f);
        }

        // iterate through cells
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {

                map.SetPixels(x * size, y * size, size, size, cols);
            }
        }
        map.Apply();


        byte[] bytes = map.EncodeToPNG();
        return bytes;
    }

}