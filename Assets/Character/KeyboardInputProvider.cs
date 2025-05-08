using UnityEngine;
public class KeyboardInputProvider : MonoBehaviour, IInputProvider
{
    private float mouseStartY = Screen.height / 2f;
    private float grabHeight = 0.5f;
    private float mouseSensitivity = 0.003f;

    public InputState GetInputState()
    {
        InputState state = new InputState();

        float h = Input.GetKey(KeyCode.A) ? -1f : (Input.GetKey(KeyCode.D) ? 1f : 0f);
        float v = Input.GetKey(KeyCode.S) ? -1f : (Input.GetKey(KeyCode.W) ? 1f : 0f);
        state.MoveDirection = new Vector3(h, 0f, v).normalized;

        bool isShift = Input.GetKey(KeyCode.LeftShift);

        state.IsJumping = Input.GetKeyDown(KeyCode.Space);
        state.IsPunchingLeft = isShift && Input.GetMouseButtonDown(0);
        state.IsPunchingRight = isShift && Input.GetMouseButtonDown(1);

        state.IsGrabbingLeft = !isShift && Input.GetMouseButton(0);
        state.IsGrabbingRight = !isShift && Input.GetMouseButton(1);

        // Adjust grab height
        if (state.IsGrabbingLeft || state.IsGrabbingRight)
        {
            float deltaY = Input.mousePosition.y - mouseStartY;
            grabHeight = Mathf.Clamp01(0.5f + deltaY * mouseSensitivity);
        }
        else
        {
            mouseStartY = Input.mousePosition.y;  // Reset for next grab
        }

        state.GrabHeight = grabHeight;
        return state;
    }
}

