using System.Net.Http.Headers;
using UnityEngine;

public enum CameraProfileType
{
    Static = 0,
    FollowTarget
}

public class CameraProfile : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] private CameraProfileType _profileType = CameraProfileType.Static;

    [Header("Follow")]
    [SerializeField] private CameraFollowable _targetToFollow = null;
    private Camera _camera;
    public float CameraSize => _camera.orthographicSize;
    public Vector3 Position => _camera.transform.position;

    [Header("Damping")]
    [SerializeField] private bool _useDampingHorizontally = false;
    [SerializeField] private float _useHorizontalDampingFactor = 5f;
    [SerializeField] private bool _useDampingVertically = false;
    [SerializeField] private float _useVerticalDampingFactor = 5f;
    
    [Header("Bounds")]
    [SerializeField] private bool _hasBounds = false;
    [SerializeField] private Rect _BoundsRect = new Rect(0f, 0f, 10f, 10f);


    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if(_camera != null )
        {
            _camera.enabled = false;
        }
    }

    public CameraProfileType ProfileType => _profileType;
    public CameraFollowable TargetToFollow => _targetToFollow;

    public bool UseDampingHorizontally => _useDampingHorizontally;
    public float HorizontalDampingFactor => _useHorizontalDampingFactor;
    public bool UseDampingVertically => _useDampingVertically;

    public float VerticalDampingFactor => _useVerticalDampingFactor;

    public bool HasBounds => _hasBounds;
    public Rect BoundsRect => _BoundsRect;
    private void OnDrawGizmosSelected()
    {
        if(!_hasBounds) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_BoundsRect.center, _BoundsRect.size);
    }
}


