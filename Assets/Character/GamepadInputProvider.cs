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
        state.GrabHeight = Input.GetAxis("RightStickY");
        float grabInput = Input.GetAxis("RightStickY");
        Debug.Log($"RightStickY: {grabInput}");

        state.IsJumping = Input.GetButtonDown("ButtonA");
        state.IsPunchingLeft = Input.GetAxis("LeftTrigger") > 0.1f;
        state.IsPunchingRight = Input.GetAxis("RightTrigger") > 0.1f;
        state.IsGrabbingLeft = Input.GetButton("LeftBumper");
        state.IsGrabbingRight = Input.GetButton("RightBumper");

        return state;
    }
}
