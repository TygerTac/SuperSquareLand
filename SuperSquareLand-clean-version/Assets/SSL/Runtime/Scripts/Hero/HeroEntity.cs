using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class HeroEntity : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")]
    [FormerlySerializedAs("_movementsSettings")]
    [SerializeField] private HeroHorizontalMovementSettings _groundHorizontalMovementSettings;
    [SerializeField] private HeroHorizontalMovementSettings _airHorizontalMovementSettings;
    private float _horizontalSpeed = 0f;
    private float _moveDirX = 0f;

    [Header("Vertical Movements")]
    private float _verticalSpeed = 0f;

    [Header("Fall")]
    [SerializeField] private HeroFallSettings _fallSettings;

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    private float _orientX = 1f;

    [Header("Dash")]
    [SerializeField] private HeroDashSettings _dashSettings;
    private float _dashTimer = 0f;
    private bool _isDashing = false;
    private bool _isJumping = false; //Dash Mid-air
    /// </summary>

    [Header("Ground")]
    [SerializeField] private GroundDetector _groundDetector;
    public bool IsTouchingGround{ get; private set;} = false;

    [Header("Wall")]
    [SerializeField] private WallDetector _wallDetector;

    public bool IsTouchingWalls{ get; private set;} = false;


    [Header("Jump")]
    [SerializeField] private HeroJumpSettings _jumpSettings;
    [SerializeField] private HeroFallSettings _jumpFallSettings;
    

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    #region Functions Move Dir
    public float GetMoveDirX()
    {
        return _moveDirX;
    }

    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;
    }
    #endregion

    private void FixedUpdate()
    {
        _ApplyGroundDetection();
        _ApplyWallDetection();
        HeroHorizontalMovementSettings HorizontalMovementSettings = _GetCurrentHorizontalMovementSettings();
        if (_AreOrientAndMovementOpposite())
        {
            _TurnBack(HorizontalMovementSettings);
        } else {
            _UpdateHorizonSpeed(HorizontalMovementSettings);
            _ChangeOrientFromHorizontalMovement();
        }
        if(IsDashing())
        {
            _UpdateDash();
        } /*else {
            if(IsTouchingWalls)
            {
            _ResetHorizontalSpeed();
            }
        } */
        if(IsJumping)
        {
            _UpdateJump();
        } else {
            if(!IsTouchingGround)
            {
                _ApplyFallGravity(_fallSettings);
            } else {
                _ResetVerticalSpeed();
            }
        }
        //_ResetHorizontalSpeed();

        _ApplyHorizontalSpeed();
        _ApplyVerticalSpeed();
    }

    

    private void _ChangeOrientFromHorizontalMovement()
    {
        if(_moveDirX == 0f) 
            return;
        _orientX = Mathf.Sign(_moveDirX);
    }

    private void _ApplyHorizontalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _horizontalSpeed * _orientX;
        _rigidbody.velocity = velocity;
    }

    private void _UpdateHorizonSpeed(HeroHorizontalMovementSettings settings)
    {
        if(_moveDirX != 0f)
        {
            _Accelerate(settings);
        } else {
            _Decelerate(settings);
        }
    }

    private HeroHorizontalMovementSettings _GetCurrentHorizontalMovementSettings()
    {
        if(IsTouchingGround)
        {
            return _groundHorizontalMovementSettings;
        } else {
            return _airHorizontalMovementSettings;
        }
    }

    private void _ApplyFallGravity(HeroFallSettings settings)
    {
        if(!IsDashing())
        {
            _verticalSpeed -= settings.FallGravity * Time.fixedDeltaTime;
            if(_verticalSpeed < -settings.fallSpeedMax)
            {
                _verticalSpeed = -settings.fallSpeedMax;
            }
        }
    }
    

    private void _ApplyVerticalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.y = _verticalSpeed;
        _rigidbody.velocity = velocity;
    }

    private void _ApplyGroundDetection()
    {
        IsTouchingGround = _groundDetector.DetectGroundNearBy();
    }

    private void _ApplyWallDetection()
    {
        IsTouchingWalls = _wallDetector.DetectLeftWallNearby() || _wallDetector.DetectRightWallNearby();
    }
    // à VERIFIER POUR FAIRE LE DASH COMME CA
    enum JumpState
    {
        NotJumping,
        JumpImpulsion,
        Falling,
    }
    private JumpState _jumpState = JumpState.NotJumping;
    private float _jumpTimer = 0f;
    private void _ResetVerticalSpeed()
    {
        _verticalSpeed = 0f;
    }
    public bool  IsJumping => _jumpState != JumpState.NotJumping;
    public void JumpStart()
    {
        if(!IsDashing())
        {
        _jumpState = JumpState.JumpImpulsion;
        _jumpTimer = 0f;
        _isJumping = true;
        }
    }

    private void _UpdateJumpStateImpulsion()
    {
        _jumpTimer += Time.fixedDeltaTime;
        if(_jumpTimer < _jumpSettings.jumpMaxDuration)
        { 
            _verticalSpeed = _jumpSettings.jumpSpeed;
        } else {
            _jumpState = JumpState.Falling;
        }
    }
    private void _UpdateJumpStateFalling()
    {
        if(!IsTouchingGround)
        {
            _ApplyFallGravity(_jumpFallSettings);
        } else {
            _ResetVerticalSpeed();
            _jumpState = JumpState.NotJumping;
        }
    }

    private void _UpdateJump()
    {
        if(!IsDashing())
        {
            switch (_jumpState)
            {
                case JumpState.JumpImpulsion:
                    _UpdateJumpStateImpulsion();
                    break;
                case JumpState.Falling:
                    _UpdateJumpStateFalling();
                    break;
            }
        }
    }

    public void StopJumpImpulsion()
    {
        _jumpState = JumpState.Falling;
    }
    public bool IsJumpImpulsing => _jumpState == JumpState.JumpImpulsion;
    public bool IsJumpMinDurationReached => _jumpTimer >= _jumpSettings.jumpMinduration;
    
    enum DashState
    {
        NotDashing,
        DashImpulsion,
        StopDash
    }

    private DashState _DashState = DashState.NotDashing;

    public void DashStart()
    {
        if(!IsDashing())
        {
        _DashState = DashState.DashImpulsion;
        _dashTimer = 0f;
        _isDashing = true;
        _verticalSpeed = 0f;

        _isJumping = false;
        _jumpState = JumpState.NotJumping;
        }
    }
    public bool IsDashing() => _isDashing;

    private void _ResetHorizontalSpeed()
    {
        _horizontalSpeed = 0f;
    }
    private void _UpdateDashStateImpulsion()
    {
        _dashTimer += Time.fixedDeltaTime;
        if(_dashTimer < _dashSettings.dashDuration)
        {
            _horizontalSpeed = _dashSettings.dashSpeed;
        } else {
            _DashState = DashState.StopDash;
        }
    }
    private void _UpdateDashStateStopDash()
    {
            _ResetHorizontalSpeed();
            _DashState = DashState.NotDashing;
            StopJumpImpulsion();
            _jumpState = JumpState.NotJumping;
            _isDashing = false;
    }
    private void _UpdateDash()
    {
        switch (_DashState)
        {
            case DashState.DashImpulsion:
                _UpdateDashStateImpulsion();
                break;
            case DashState.StopDash:
                _UpdateDashStateStopDash();
                break;
        }
    }
    private void _Accelerate(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed += settings.acceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed > settings.speedMax) 
        {
            _horizontalSpeed = settings.speedMax;
        }
    }
    
    private void _Decelerate(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed -= settings.deceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f) 
        {
            _horizontalSpeed = 0f;
        }
    }

    private void _TurnBack(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed -= settings.turnBackFrictions * Time.fixedDeltaTime;
        if(_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
            _ChangeOrientFromHorizontalMovement();
        }
    }
    
    private bool _AreOrientAndMovementOpposite()
    {
        return _moveDirX * _orientX < 0f;
    }
    

    private void Update()
    {
        _UpdateOrientVisual();
    }

    private void _UpdateOrientVisual()
    {
        Vector3 newScale = _orientVisualRoot.localScale;
        newScale.x = _orientX;
        _orientVisualRoot.localScale = newScale;
    }

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        if(IsTouchingGround){
            GUILayout.Label("OnGround");
        } else {
            GUILayout.Label("InAir");
        }
        if(!IsTouchingWalls){
            GUILayout.Label("NoWalls");
        } else {
            GUILayout.Label("TouchingWalls");
        }
        GUILayout.Label($"JumpState = {_jumpState}");
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.Label($"Vertical Speed = {_verticalSpeed}");
        GUILayout.EndVertical();
    }
}