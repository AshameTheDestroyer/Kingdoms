using UnityEngine;

[RequireComponent (typeof(Renderer))]
public class PerlinNoise : MonoBehaviour
{
    [SerializeField] private Vector2Int _size = Vector2Int.one * 256;
    [SerializeField] private Vector2Int _offset = Vector2Int.one * 100;
    [SerializeField] private float _scale = 20;

    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        UpdateMainTexture();
    }

    private void UpdateMainTexture()
    {
        _renderer.material.mainTexture = GenerateTexture();
    }

    private Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(_size.x, _size.y);

        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                float noiseX = (float)x / _size.x * _scale + _offset.x;
                float noiseY = (float)y / _size.y * _scale + _offset.y;

                float sample = Mathf.PerlinNoise(noiseX, noiseY);
                Color colour = new Color(sample, sample, sample);

                texture.SetPixel(x, y, colour);
            }
        }

        texture.Apply();
        return texture;
    }
}
