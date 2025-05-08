using UnityEngine;
public struct InputState
{
    public Vector3 MoveDirection;
    public Vector3 InputDirection;
    public bool IsJumping;
    public bool IsPunchingLeft;
    public bool IsPunchingRight;
    public bool IsGrabbingLeft;
    public bool IsGrabbingRight;
    public float GrabHeight;  // for grab height
}
