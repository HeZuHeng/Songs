using UnityEngine;
using System.Collections;

public class Twist : MonoBehaviour
{
    public float twistAngle = 0;

    private Material mat;
    void Start()
    {
        mat = new Material(Shader.Find("Unlit/screenTwist"));
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        mat.SetFloat("_Twist", twistAngle);

        Graphics.Blit(source, destination, mat);
    }
}