using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    public Terrain terrain;
    public Transform water;
    public Material[] skys;
    public Light sceneLight;
    public Transform audioParent;
    private void Awake()
    {
        if(terrain == null)
        {
            terrain = GetComponentInChildren<Terrain>();
        }
        if(sceneLight == null)
        {
            sceneLight = GetComponentInChildren<Light>();
        }
        if(water == null)
        {
            AQUAS_Reflection qUAS_Reflection = GetComponentInChildren<AQUAS_Reflection>();
            if(qUAS_Reflection != null)
            {
                water = qUAS_Reflection.transform;
            }
        }
        Transform skysParent = transform.Find("Skys");
        if(skysParent != null)
        {
            skys = new Material[skysParent.childCount];
            for (int i = 0; i < skysParent.childCount; i++)
            {
                skys[i] = skysParent.GetChild(i).GetComponent<MeshRenderer>().sharedMaterial;
            }
        }
        if(audioParent == null)
        {
             AudioSource a = GetComponentInChildren<AudioSource>();
            if(a != null)
            {
                audioParent = a.transform;
            }
        }
    }

    public void SetDetailObjects(float val)
    {
      if(terrain != null)  terrain.detailObjectDensity = val;
    }

    public void SetSky(int index,float intensity)
    {
        if(index < skys.Length)
        {
            RenderSettings.skybox = skys[index];
        }
        if(sceneLight != null)
        {
            sceneLight.intensity = intensity;
        }
    }

    public void SetWater(bool val)
    {
        if (water != null) water.gameObject.SetActive(val);
    }
}
