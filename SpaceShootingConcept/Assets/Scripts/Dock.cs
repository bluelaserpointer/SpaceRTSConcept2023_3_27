using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Dock : MonoBehaviour
{
    [SerializeField]
    Collider _mainColldier;

    List<DockPort> _ports;
    private void Awake()
    {
        _ports = transform.FindComponentsInDirectChildren<DockPort>();
    }
    public DockPort SuggestPort()
    {
        return _ports.Find(port => port.Avaliable);
    }
    public DockPort SuggestPortByRaySelect(Ray ray, float rayDistance)
    {
        if(_mainColldier.Raycast(ray, out RaycastHit hitInfo, rayDistance))
        {
            float minDistance = float.MaxValue;
            DockPort closestPort = null;
            foreach(DockPort port in _ports)
            {
                float distance = Vector3.Distance(hitInfo.point, port.transform.position);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    closestPort = port;
                }
            }
            return closestPort;
        }
        return null;
    }
}
