using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallDetector : MonoBehaviour //Ground Detector mais pour le wall
{
[Header("Detection")]
[SerializeField] private Transform[] _detectionPoints;
[SerializeField] private float _detectionLength = 0.1f;
[SerializeField] private LayerMask _WallLayerMask;

public bool DetectLeftWallNearby()
{
    foreach(Transform detectionPoint in _detectionPoints)
    {
        RaycastHit2D hitResult = Physics2D.Raycast(
            detectionPoint.position,
            Vector2.left,
            _detectionLength,
            _WallLayerMask
        );

        if(hitResult.collider != null)
        {
            return true;
        }
    }
    return false;
}

public bool DetectRightWallNearby()
{
    foreach(Transform detectionPoint in _detectionPoints)
    {
        RaycastHit2D hitResult = Physics2D.Raycast(
            detectionPoint.position,
            Vector2.right,
            _detectionLength,
            _WallLayerMask
        );

        if(hitResult.collider != null)
        {
            return true;
        }
    }
    return false;
}
}
