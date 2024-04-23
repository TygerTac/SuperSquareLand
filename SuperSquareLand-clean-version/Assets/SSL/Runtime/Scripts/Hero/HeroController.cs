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
        if (Input.GetKeyDown(KeyCode.E))
        {
            _entity._Dash();
        }
        //HandleDashInput();
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
    /*
    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _entity._Dash();
        }
    }
    */
}