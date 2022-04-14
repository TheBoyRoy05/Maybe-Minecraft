using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu()]
public class MapSettings : ScriptableObject{
    [Range (2, 256)] public int res = 69;

    public float scale = 1.0f;
    public float strength = 1;
    [Range(1, 8)] public int numLayers = 1;
    public float baseRoughness = 1;
    public float roughness = 2;
    [Range(-1, 1)] public float persistence = .5f;
    public Vector3 center;
    public float min;

    public Material mapMaterial;
    public Gradient gradient;
}
