using UnityEngine;
public class KeyboardInputProvider : MonoBehaviour, IInputProvider
{
    public InputState GetInputState()
    {
        InputState state = new InputState();

        // WASD
        float h = Input.GetKey(KeyCode.A) ? -1f : (Input.GetKey(KeyCode.D) ? 1f : 0f);
        float v = Input.GetKey(KeyCode.S) ? -1f : (Input.GetKey(KeyCode.W) ? 1f : 0f);
        state.MoveDirection = new Vector3(h, 0f, v).normalized;

        state.IsJumping = Input.GetKeyDown(KeyCode.Space);
        state.IsPunchingLeft = Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0);
        state.IsPunchingRight = Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1);
        state.IsGrabbingLeft = !Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0);
        state.IsGrabbingRight = !Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(1);
        state.GrabHeight = Input.mousePosition.y;

        return state;
    }
}
