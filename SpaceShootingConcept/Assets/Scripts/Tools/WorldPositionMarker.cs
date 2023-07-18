using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldPositionMarker
{
    public static void Mark(GameObject marker, Camera camera, Vector3 targetPosition)
    {
        Vector3 screenPos = camera.WorldToScreenPoint(targetPosition);
        if(screenPos.z < 0)
            marker.SetActive(false);
        else
        {
            marker.SetActive(true);
            marker.transform.position = screenPos.Set(z: 0);
        }
    }
}
