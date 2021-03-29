using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPAtmosphere : MonoBehaviour
{
    public Material mat;
    public Light light;

    public Vector3 wavelengths = new Vector3(700, 530, 460);
    public float scatteringStrength = 20;

    public float planetRadius = 4096;
    [Range(1.1f, 2.0f)]
    public float atmosphereScale = 1.5f;
    [Range(-3.0f, 20.0f)]
    public float densityFallofff = 0.25f;
    [Range(0.5f, 2.0f)]
    public float intensity = 1.0f;

    [Range(0, 20)]
    public int inScatteringPoints = 10;
    [Range(0, 20)]
    public int opticalDepthPoints = 10;

    private Vector3 _Scatter;

    public bool rotate = false;
    public float cameraRotateSpeed = 50;

    private void Start()
    {
        RecalcScatter();
        UpdateSettings();
    }

    void Update()
    {
        if (rotate)
            this.transform.RotateAround(Vector3.zero, Vector3.up, cameraRotateSpeed * Time.deltaTime);
    }

    void UpdateSettings()
    {
        mat.SetVector("dirToSun", -light.transform.forward);
        mat.SetVector("planetCenter", Vector3.zero);
        mat.SetVector("scatteringCoefficients", _Scatter * scatteringStrength);

        mat.SetFloat("planetRadius", planetRadius);
        mat.SetFloat("atmosphereRadius", planetRadius * atmosphereScale);
        mat.SetFloat("densityFallofff", densityFallofff);

        mat.SetInt("numInScatteringPoints", inScatteringPoints);
        mat.SetInt("numOpticalDepthPoints", opticalDepthPoints);
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            RecalcScatter();
            UpdateSettings();
        }
    }

    void RecalcScatter()
    {
        _Scatter = Vector3.zero;
        _Scatter.x = Mathf.Pow(400 / wavelengths.x, 4);
        _Scatter.y = Mathf.Pow(400 / wavelengths.y, 4);
        _Scatter.z = Mathf.Pow(400 / wavelengths.z, 4);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}
