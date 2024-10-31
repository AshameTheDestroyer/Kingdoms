using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] int _depth = 24;
    [SerializeField] Vector2Int _size = Vector2Int.one * 256;
    [SerializeField] private Vector2 _offset = Vector2.one * 100;
    [SerializeField] private float _scale = 20;
    [SerializeField] private float _offsetSpeed = 20;

    private Terrain _terrain;

    private void Start()
    {
        _terrain = GetComponent<Terrain>();
    }

    private void Update()
    {
        _terrain.terrainData = GenerateTerrain(_terrain.terrainData);
        _offset.x += _offsetSpeed * Time.deltaTime;
        _offset.y += _offsetSpeed * Time.deltaTime;
    }

    private TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = _size.x + 1;
        terrainData.size = new Vector3(_size.x, _depth, _size.y);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    private float[,] GenerateHeights()
    {
        float[,] heights = new float[_size.x, _size.y];
        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                float noiseX = (float)x / _size.x * _scale + _offset.x;
                float noiseY = (float)y / _size.y * _scale + _offset.y;

                heights[x, y] = Mathf.PerlinNoise(noiseX, noiseY);
            }
        }

        return heights;
    }
}
