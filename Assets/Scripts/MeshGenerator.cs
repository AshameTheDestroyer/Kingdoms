using System;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent (typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    [SerializeField] float _depth = 12;
    [SerializeField] uint _seed = 0;
    [SerializeField] Vector2Int _size = Vector2Int.one * 256;
    [SerializeField] float _islandCoherence = 8;
    [SerializeField, Range(0f, 0.2f)] float _islandSpread = 0.075f;
    [SerializeField] private float _scale = 64;
    [SerializeField] int _noiseOctaves = 16;
    [SerializeField] float _noiseScale = 8;

    [SerializeField] private Gradient colourGradient;

    private Mesh _mesh;
    private MeshCollider _meshCollider;
    private int[] _triangles;
    private Vector3[] _vertices;

    [InspectorButton(nameof(Start), ButtonWidth = 200)] public bool initializeTriangles;
    [InspectorButton(nameof(UpdateVertices), ButtonWidth = 200)] public bool updateMesh;
    [InspectorButton(nameof(RandomizeSeed), ButtonWidth = 200)] public bool randomizeSeed;

    private void Start()
    {
        _meshCollider = GetComponent<MeshCollider>();
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();

        if (_seed == 0) { RandomizeSeed(); }

        InitializeTriangles();
        UpdateVertices();
    }

    private void RandomizeSeed()
    {
        _seed = Unity.Mathematics.Random.CreateFromIndex((uint)DateTime.Now.Millisecond).NextUInt();
    }

    private void InitializeTriangles()
    {
        _triangles = new int[_size.x * _size.y * 6];
        for (int vertexIndex = 0, triangleIndex = 0, x = 0; x < _size.x; x++, vertexIndex++)
        {
            for (int y = 0; y < _size.y; y++, vertexIndex++, triangleIndex += 6)
            {
                _triangles[triangleIndex] = vertexIndex;
                _triangles[triangleIndex + 5] = vertexIndex + _size.x + 2;
                _triangles[triangleIndex + 1] = _triangles[triangleIndex + 4] = vertexIndex + 1;
                _triangles[triangleIndex + 2] = _triangles[triangleIndex + 3] = vertexIndex + _size.x + 1;
            }
        }
    }

    private void UpdateVertices()
    {
        _vertices = new Vector3[(_size.x + 1) * (_size.y + 1)];
        Vector3 origin = new Vector3(Mathf.Sqrt(_seed), Mathf.Sqrt(_seed));

        for (int i = 0, x = 0; x < _size.x + 1; x++)
        {
            for (int y = 0; y < _size.y + 1; y++, i++)
            {
                float noiseX = x * _scale;
                float noiseY = y * _scale;

                float sample = GenerateNoise(noiseX, noiseY, origin);
                _vertices[i] = new Vector3(x - _size.x / 2f, sample * _depth, y - _size.y / 2f);
            }
        }

        UpdateMesh();
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

        return Mathf.Max(0, sample - FallOfMap(x, y));
    }

    private float FallOfMap(float x, float y)
    {
        float x_ = Mathf.Pow((x / _scale - _size.x / 2f) / _size.x, 2);
        float y_ = Mathf.Pow((y / _scale - _size.y / 2f) / _size.y, 2);
        return  (x_ + y_ - _islandSpread) * _islandCoherence;
    }

    private void UpdateMesh()
    {
        _mesh.Clear();

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.colors32 = _vertices.Select(vertex => (Color32)colourGradient.Evaluate(vertex.y / _depth)).ToArray();

        _mesh.RecalculateNormals();
        _meshCollider.sharedMesh = _mesh;
    }
}
