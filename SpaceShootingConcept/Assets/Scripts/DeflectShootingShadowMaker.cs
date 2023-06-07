using IzumiTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class DeflectShootingShadowMaker : MonoBehaviour
{
    public GameObject _targetObject;
    [SerializeField]
    Camera _mainCamera;

    [SerializeField]
    RawImage _dsShadowView;
    [SerializeField]
    Camera _shadowCamera;
    [SerializeField]
    ReuseNest<MeshFilter> _shadowMeshFilterNest;

    [HideInInspector]
    Vector3 _estimateTargetPosition;

    float fovRatio;

    public void Init()
    {
        _shadowMeshFilterNest.InactivateAll();
        _shadowMeshFilterNest.nest.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        foreach (var meshFilter in _targetObject.GetComponentsInChildren<MeshFilter>())
        {
            var shadow = _shadowMeshFilterNest.Get();
            shadow.mesh = meshFilter.mesh;
            shadow.transform.localPosition = _targetObject.transform.InverseTransformPoint(meshFilter.transform.position);
            shadow.transform.localRotation = Quaternion.Inverse(_targetObject.transform.rotation) * meshFilter.transform.rotation;
            shadow.transform.localScale = meshFilter.transform.lossyScale;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        fovRatio = _dsShadowView.rectTransform.Height() / Screen.height;
        Init();
    }

    void LateUpdate()
    {
        _shadowCamera.fieldOfView = _mainCamera.fieldOfView * fovRatio;
        _shadowMeshFilterNest.nest.transform.position = _shadowCamera.transform.TransformPoint(
            _mainCamera.transform.InverseTransformPoint(_estimateTargetPosition));
        _shadowMeshFilterNest.nest.rotation = Quaternion.Inverse(_mainCamera.transform.rotation) * _targetObject.transform.rotation;
    }
    public void SetEstimateTargetPosition(Vector3 position)
    {
        _estimateTargetPosition = position;
    }
}
