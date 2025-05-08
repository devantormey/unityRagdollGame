using UnityEngine;
using System.IO;

public class GamepadInputProvider : MonoBehaviour, IInputProvider
{
    public string configFileName = "GamepadBindings.json";
    private GamepadBindings bindings;

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

    public InputState GetInputState()
    {
        InputState state = new InputState();

        // Basic hardcoded mapping for now
        state.MoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        Debug.Log($"Left Stick input: {state.MoveDirection}");
        
        //Grabheigh stuff
        float rawGrab = Input.GetAxis("RightStickY");  // Range: -1 to 1
        float grabHeight = Mathf.Clamp01((rawGrab + 1f) * 0.5f);  // Now: 0 to 1
        Debug.Log($"RightStickY: {grabHeight}");
        state.GrabHeight = grabHeight;

        state.IsJumping = Input.GetButtonDown("ButtonA");
        state.IsPunchingLeft = Input.GetAxis("LeftTrigger") > 0.4f;
        state.IsPunchingRight = Input.GetAxis("RightTrigger") > 0.4f;
        state.IsGrabbingLeft = Input.GetButton("LeftBumper");
        state.IsGrabbingRight = Input.GetButton("RightBumper");

        return state;
    }
}
