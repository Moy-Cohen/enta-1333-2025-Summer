using UnityEngine;
using UnityEditor;
using System.IO;

public class CameraIconSaver
{
    [MenuItem("Tools/Save Camera Icon")]
    static void SaveCameraIcon()
    {
        Camera cam = Selection.activeGameObject?.GetComponent<Camera>();
        if (cam == null || cam.targetTexture == null)
        {
            Debug.LogError("Select a Camera with a RenderTexture assigned!");
            return;
        }

        RenderTexture rt = cam.targetTexture;
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        string path = EditorUtility.SaveFilePanel("Save Icon", "Assets/Icons/", "UnitIcon.png", "png");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.Refresh();
            Debug.Log("Saved icon to: " + path);
        }

        RenderTexture.active = null;
    }
}