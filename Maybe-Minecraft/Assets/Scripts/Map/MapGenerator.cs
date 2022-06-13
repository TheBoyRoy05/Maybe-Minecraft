using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour{
    [SerializeField, HideInInspector]
    MeshFilter meshfil;
    Mesh m;
    Noise n;

    Texture2D texture;
    const int textureRes = 50;

    public float MinValue = float.MaxValue;
    public float MaxValue = float.MinValue;

    public MapSettings settings;
    [HideInInspector] public bool settingsFoldout;
    public bool autoUpdate = true;

    void Start(){
        GenerateMap();
    }

    void Initialize(){
        n = new Noise();

        if(meshfil == null){
            GameObject meshObj = new GameObject("mesh");
            meshObj.transform.parent = transform;
            meshObj.AddComponent<MeshRenderer>();
            meshfil = meshObj.AddComponent<MeshFilter>();
            meshfil.sharedMesh = new Mesh();
            meshObj.AddComponent<MeshCollider>();
            meshObj.layer = LayerMask.NameToLayer("Ground");
        }
        meshfil.GetComponent<MeshRenderer>().sharedMaterial = settings.mapMaterial;
        m = meshfil.sharedMesh;
        GetComponentInChildren<MeshCollider>().sharedMesh = m;

        if(texture == null){
            texture = new Texture2D(textureRes, 1);
        }
    }

    public void GenerateMesh(){
        Vector3[] vertices = new Vector3[settings.res * settings.res];
        int[] triangles = new int[(settings.res - 1) * (settings.res - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < settings.res; y++){
            for (int x = 0; x < settings.res; x++){
                int i = x + y * settings.res;
                Vector2 percent = new Vector2(x, y) / (settings.res - 1);
                Vector3 point = percent.x * settings.scale * Vector3.right + percent.y * settings.scale * Vector3.forward;
                float elevation = Evaluate(point);
                vertices[i] = point + elevation * Vector3.up;

                if (x != settings.res - 1 && y != settings.res - 1){
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + settings.res;
                    triangles[triIndex + 2] = i + settings.res + 1;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + settings.res + 1;
                    triangles[triIndex + 5] = i + 1;
                    triIndex += 6;
                }
            }
        }
        m.Clear();
        m.vertices = vertices;
        m.triangles = triangles;
        m.RecalculateNormals();
    }

    public void UpdateColors(){
        Color[] colors = new Color[textureRes];
        for (int i = 0; i < textureRes; i++){
            colors[i] = settings.gradient.Evaluate(i / (textureRes - 1f));
        }
        texture.SetPixels(colors);
        texture.Apply();
        settings.mapMaterial.SetTexture("_texture", texture);
    }
    
    public float Evaluate(Vector3 point){
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < settings.numLayers; i++){
            float v = n.Evaluate(point * frequency + settings.center);
            noiseValue += (v+1) * .5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }
        noiseValue = (Mathf.Max(noiseValue, settings.min)  - Mathf.Max(settings.min, 0)) * settings.strength;
        setMinMax(noiseValue);
        return noiseValue;
    }

    public void setMinMax(float x){
        if(x < MinValue) MinValue = x;
        if(x > MaxValue) MaxValue = x;
    }

    public void UpdateElevation(){
        settings.mapMaterial.SetVector("_elevationMinMax", new Vector4(MinValue, MaxValue));
    }
    
    public void GenerateMap(){
        Initialize();
        GenerateMesh();
        UpdateElevation();
        UpdateColors();
    }

    public void OnMapSettingsUpdated(){
        if (autoUpdate){
            MinValue = float.MaxValue;
            MaxValue = float.MinValue;
            GenerateMap();
        }
    }
}