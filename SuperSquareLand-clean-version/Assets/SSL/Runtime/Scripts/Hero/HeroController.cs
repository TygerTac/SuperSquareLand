using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private HeroEntity _entity;
    private bool _entityWastouchingGround = false;

    [Header("Coyote Time")]
    [SerializeField] private float _coyoteTimeDuration = 0.2f;
    private float _coyoteTimeCountdown = 1f;

    [Header("Jump Buffering")]
    [SerializeField] private float _jumpBufferDuration = 0.2f;
    private float _jumpBufferTimer = 0f;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"Jump Buffer Timer = {_jumpBufferTimer}");
        GUILayout.EndVertical();
    }
    private void Start()
    {
        _CancelJumpBuffer();
    }

    private void Update()
    {
        _entity.SetMoveDirX(GetInputMoveX());

        if(_EntityHasExitGround())
        {
            _ResetCoyoteTime();
        } else {
            _UpdateCoyoteTime();
        }

        if (_GetInputDownJump())
        {
            if ((_entity.IsTouchingGround || _IsCoyoteTimeActive()) && !_entity.IsJumping)
            {
                _entity.JumpStart();
            } else {
                _ResetJumpBuffer();
            }
        }

        if(IsJumpBufferActive())
        {
            if((_entity.IsTouchingGround || _IsCoyoteTimeActive()) && !_entity.IsJumping)
            {
                _entity.JumpStart();
            }
        }

        if (_entity.IsJumpImpulsing)
        {
            if(!_GetInputJump() && _entity.IsJumpMinDurationReached)
            {
                _entity.StopJumpImpulsion();
            }
        }
        if (_GetInputDash())
        {    
            _entity.DashStart();    
        }
        _entityWastouchingGround = _entity.IsTouchingGround;
    }

    private float GetInputMoveX()
    {
        float InputMoveX = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q))
        { //Negative veut dire à gauche
            InputMoveX = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        { //Positive veut dire à droite
            InputMoveX = 1f;
        }
        return InputMoveX;
    }

    private bool _GetInputDownJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
    private bool _GetInputJump()
    {
        return Input.GetKey(KeyCode.Space);
    }
    private bool _GetInputDash()
    {
        return Input.GetKey(KeyCode.E);
    }

    private void _ResetJumpBuffer()
    {
        _jumpBufferTimer = 0f;
    }

    private bool IsJumpBufferActive()
    {
        return _jumpBufferTimer < _jumpBufferDuration;
    }

    private void _UpdateJumpBuffer()
    {
        if(!IsJumpBufferActive()) return;
        _jumpBufferTimer += Time.deltaTime;
    }

    private void _CancelJumpBuffer()
    {
        _jumpBufferTimer = _jumpBufferDuration;
    }

    private void _UpdateCoyoteTime()
    {
        if(!_IsCoyoteTimeActive()) return;
        _coyoteTimeCountdown -= Time.deltaTime;
    }
    private bool _IsCoyoteTimeActive()
    {
        return _coyoteTimeCountdown > 0f;
    }

    private void _ResetCoyoteTime()
    {
        _coyoteTimeCountdown = _coyoteTimeDuration;
    }

    private bool _EntityHasExitGround()
    {
        return _entityWastouchingGround && !_entity.IsTouchingGround;
    }

}