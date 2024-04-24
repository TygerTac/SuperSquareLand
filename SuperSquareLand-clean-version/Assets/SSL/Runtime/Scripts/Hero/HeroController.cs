using UnityEngine;

public class HeroController : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private HeroEntity _entity;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.EndVertical();
    }

    private void Update()
    {
        _entity.SetMoveDirX(GetInputMoveX());

        if (_GetInputDownJump())
        {
            if (_entity.IsTouchingGround && !_entity.IsJumping)
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
}