using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    [SerializeField] int _depth = 24;
    [SerializeField] Vector2Int _size = Vector2Int.one * 256;
    [SerializeField] private Vector2 _offset = Vector2.one * 100;
    [SerializeField] private float _scale = 20;
    [SerializeField] private float _offsetSpeed = 20;

    private Mesh _mesh;
    private int[] _triangles;
    private Vector3[] _vertices;

    private void Start()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        CreateShape();
        UpdateMesh();
    }

    private void CreateShape()
    {
        _vertices = new Vector3[(_size.x + 1) * (_size.y + 1)];

        for (int i = 0, x = 0; x < _size.x + 1; x++)
        {
            for (int y = 0; y < _size.y + 1; y++, i++)
            {
                float depth = Mathf.PerlinNoise((x - _size.x / 2f) * 0.3f, (y - _size.y / 2f) * 0.3f) * 2;
                _vertices[i] = new Vector3(x - _size.x / 2f, depth, y - _size.y / 2f);
            }
        }

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

    private void UpdateMesh()
    {
        _mesh.Clear();

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;

        _mesh.RecalculateNormals();
    }
}
