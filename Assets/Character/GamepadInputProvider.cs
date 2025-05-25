using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;




public class GamepadInputProvider : MonoBehaviour, IInputProvider
{
    public string configFileName = "GamepadBindings.json";
    private GamepadBindings bindings;

    private int jumpcount = 0;
    //Input System Mapping:
    InputAction moveAction;
    InputAction jumpAction;

    // void Awake()
    // {
    //     string path = Path.Combine(Application.streamingAssetsPath, configFileName);
    //     if (File.Exists(path))
    //     {
    //         string json = File.ReadAllText(path);
    //         bindings = JsonUtility.FromJson<GamepadBindings>(json);
    //     }
    //     else
    //     {
    //         Debug.LogError("Gamepad config file not found at: " + path);
    //         bindings = new GamepadBindings(); // fallback
    //     }
    // }
    private void Start()
    {
        // Find the references to the "Move" and "Jump" actions
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }


    public InputState GetInputState()
    {
        InputState state = new InputState();
        
        // Vector2 moveValue = moveAction.ReadValue<Vector2>();
        // state.MoveDirection = new Vector3(moveValue[0], 0f, moveValue[1]);

        // bool jumpButtonState = jumpAction.IsPressed();
        
        // if(jumpButtonState & !state.IsJumping){
        //     state.IsJumping = true;
        //     jumpcount++;
        //     Debug.Log("JumpCount: " + jumpcount);
        // }
        










        //OLD CODE
        // Basic hardcoded mapping for now
        state.MoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        // Debug.Log($"Left Stick input: {state.MoveDirection}");
        
        //Grabheigh stuff
        float rawGrab = Input.GetAxis("RightStickY");  // Range: -1 to 1
        float grabHeight = Mathf.Clamp01((rawGrab + 1f) * 0.5f);  // Now: 0 to 1
        float attackProgress = Mathf.Clamp01((rawGrab + 1f) * 0.5f);  // Now: 0 to 1
        // Debug.Log($"RightStickY: {grabHeight}");
        state.GrabHeight = grabHeight;
        state.AttackProgress = attackProgress;

        state.IsJumping = Input.GetButtonDown("ButtonA");
        // state.IsPunchingLeft = Input.GetAxis("LeftTrigger") > 0.4f;
        // state.IsPunchingRight = Input.GetAxis("RightTrigger") > 0.4f;
        state.IsAttacking = Input.GetAxis("RightTrigger") > 0.4f;
        
        state.IsGrabbingLeft = Input.GetButton("LeftBumper");
        state.IsGrabbingRight = Input.GetButton("RightBumper");

        return state;
    }
}
