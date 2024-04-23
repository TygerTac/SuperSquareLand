using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
[Header("Detection")]
[SerializeField] private Transform[] _detectionPoints;
[SerializeField] private float _detectionLength = 0.1f;
[SerializeField] private LayerMask _groundLayerMask;
}
