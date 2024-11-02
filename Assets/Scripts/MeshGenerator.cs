using System;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField] private float _landDepth = 12;
    [SerializeField] private float _waterDepth = 1;
    [SerializeField] private uint _seed = 0;
    [SerializeField] private Vector2Int _size = Vector2Int.one * 256;
    [SerializeField] private float _islandCoherence = 8;
    [SerializeField, Range(0f, 0.2f)] private float _islandSpread = 0.075f;
    [SerializeField] private float _scale = 64;
    [SerializeField] private int _noiseOctaves = 16;
    [SerializeField] private float _noiseScale = 8;

    [SerializeField] private Gradient _landColourGradient;
    [SerializeField] private Gradient _waterColourGradient;

    [SerializeField] private float _waterRise = 1;
    [SerializeField] private float _waveSpeed = 0.5f;

    [SerializeField] private MeshFilter _landMeshFilter;
    [SerializeField] private MeshFilter _waterMeshFilter;

    [InspectorButton(nameof(GenerateWorld), ButtonWidth = 200)] public bool _generateWorld;
    [InspectorButton(nameof(RandomizeSeed), ButtonWidth = 200)] public bool _randomizeSeed;

    private Mesh LandMesh { get => _landMeshFilter.mesh; set => _landMeshFilter.mesh = value; }
    private Mesh WaterMesh { get => _waterMeshFilter.mesh; set => _waterMeshFilter.mesh = value; }

    private void Update()
    {
        WaterMesh.vertices = WaveVertices(WaterMesh.vertices);
    }
    public void GenerateWorld()
    {
        if (_seed == 0) { RandomizeSeed(); }

        LandMesh = GenerateTerrain(_landColourGradient, _landDepth, _landMeshFilter.GetComponent<MeshCollider>());
        WaterMesh = GenerateTerrain(_waterColourGradient, _waterDepth, _waterMeshFilter.GetComponent<MeshCollider>());
    }

    private Mesh GenerateTerrain(Gradient colourGradient, float depth, MeshCollider meshCollider)
    {
        var mesh = new Mesh();

        mesh.vertices = GenerateVertices(depth);
        mesh.triangles = GenerateTriangles();
        mesh.colors32 = mesh.vertices.Select(vertex => (Color32)colourGradient.Evaluate(vertex.y / depth)).ToArray();

        mesh.RecalculateNormals();
        if (meshCollider != null) { meshCollider.sharedMesh = mesh; }

        return mesh;
    }

    private Vector3[] GenerateVertices(float depth)
    {
        var vertices = new Vector3[(_size.x + 1) * (_size.y + 1)];
        Vector3 origin = new Vector3(Mathf.Sqrt(_seed), Mathf.Sqrt(_seed));

        for (int i = 0, x = 0; x < _size.x + 1; x++)
        {
            for (int y = 0; y < _size.y + 1; y++, i++)
            {
                float noiseX = x * _scale;
                float noiseY = y * _scale;

                float sample = GenerateNoise(noiseX, noiseY, origin);
                vertices[i] = new Vector3(x - _size.x / 2f, sample * depth, y - _size.y / 2f);
            }
        }

        return vertices;
    }

    private int[] GenerateTriangles()
    {
        var triangles = new int[_size.x * _size.y * 6];
        for (int vertexIndex = 0, triangleIndex = 0, x = 0; x < _size.x; x++, vertexIndex++)
        {
            for (int y = 0; y < _size.y; y++, vertexIndex++, triangleIndex += 6)
            {
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 5] = vertexIndex + _size.x + 2;
                triangles[triangleIndex + 1] = triangles[triangleIndex + 4] = vertexIndex + 1;
                triangles[triangleIndex + 2] = triangles[triangleIndex + 3] = vertexIndex + _size.x + 1;
            }
        }

        return triangles;
    }

    private void RandomizeSeed()
    {
        _seed = Unity.Mathematics.Random.CreateFromIndex((uint)DateTime.Now.Millisecond).NextUInt();
    }

    private float GenerateNoise(float x, float y, Vector3 origin)
    {
        float sample = 0;

        for (float noiseScale = _noiseScale, opacity = 1, i = 0; i < _noiseOctaves; i++, noiseScale /= 2, opacity *= 2)
        {
            float x_ = (x / (noiseScale * _size.x)) + origin.x;
            float y_ = (y / (noiseScale * _size.y)) + origin.y;
            float z = noise.cnoise(new float2(x_, y_));
            sample += Mathf.InverseLerp(0, 1, z) / opacity;
        }

        return sample - FallOfMap(x, y);
    }

    private float FallOfMap(float x, float y)
    {
        float x_ = Mathf.Pow((x / _scale - _size.x / 2f) / _size.x, 2);
        float y_ = Mathf.Pow((y / _scale - _size.y / 2f) / _size.y, 2);
        return  (x_ + y_ - _islandSpread) * _islandCoherence;
    }

    private Vector3[] WaveVertices(Vector3[] vertices)
    {
        for (int i = 0, x = 0; x < _size.x + 1; x++)
        {
            for (int y = 0; y < _size.y + 1; y++, i++)
            {
                float noiseX = (float)x / _size.x * _scale + _waveSpeed * Time.time;
                float noiseY = (float)y / _size.y * _scale + _waveSpeed * Time.time;

                Vector3 vertex = vertices[i];
                vertex.y = (Mathf.PerlinNoise(noiseX, noiseY) * 2 - 1) * _waterRise;
                vertices[i] = vertex;
            }
        }

        return vertices;
    }
}
