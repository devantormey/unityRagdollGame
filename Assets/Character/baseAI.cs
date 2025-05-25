using UnityEngine;

public class baseAI : MonoBehaviour
{

    private void Start()
    {
        
    }


    public InputState GetInputState()
    {
        InputState state = new InputState();    
        return state;
    }
}
