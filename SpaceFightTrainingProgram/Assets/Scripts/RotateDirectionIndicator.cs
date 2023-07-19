using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class RotateDirectionIndicator : MonoBehaviour
{
    [SerializeField]
    Color _color;
    [SerializeField]
    float _texUVRollSpeed;
    [SerializeField]
    Renderer _renderer;

    bool _isClockwise;
    Material _material;
    private void Awake()
    {
        _material = _renderer.material;
        _material.color = _color;
        _isClockwise = true; // the texture's default facing
    }

    void Update()
    {
        _material.mainTextureOffset += Vector2.left * _texUVRollSpeed * Time.deltaTime; // the texture's default direction
    }

    public void SetRotateDirection(bool isClockwise)
    {
        if(_isClockwise != isClockwise)
        {
            _isClockwise = isClockwise;
            Vector3 scale = _renderer.transform.localScale;
            scale.y *= -1;
            _renderer.transform.localScale = scale;
        }
    }
}
