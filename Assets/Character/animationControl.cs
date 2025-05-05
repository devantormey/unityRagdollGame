using UnityEngine;

public class animationControl : MonoBehaviour
{
    Animator animator;
    private float rotation_speed = 150f;
    private bool GrabLeft = false;
    private bool GrabRight = false;

    private float grabHeight = 0.5f; // start at mid
    private float mouseStartY;
    private float mouseSensitivity = 0.003f; // tweak as needed

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool isPressingW = Input.GetKey(KeyCode.W);
        bool isPressingLeft = Input.GetKey(KeyCode.A);
        bool isPressingRight = Input.GetKey(KeyCode.D);
        animator.SetBool("isWalking", isPressingW);
        
        // Rotate
        if (isPressingLeft)
        {
            transform.Rotate(Vector3.down * rotation_speed * Time.deltaTime);
        }
        if (isPressingRight)
        {
            transform.Rotate(Vector3.up * rotation_speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("PunchLeft");
            }
            else if (Input.GetMouseButtonDown(1))
            {
                animator.SetTrigger("PunchRight");
            }
        }else{

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetBool("Grab",true);
                GrabLeft = true;
                mouseStartY = Input.mousePosition.y;
                // Debug.Log("Setting Grab True");
            }
            else if (Input.GetMouseButtonDown(1))
            {
                animator.SetBool("Grab",true);
                GrabRight = true;
                mouseStartY = Input.mousePosition.y;
                // Debug.Log("Setting Grab True");
            }
        }



        if (GrabLeft || GrabRight)
        {
            float mouseDeltaY = Input.mousePosition.y - mouseStartY;
            grabHeight = Mathf.Clamp01(0.5f + mouseDeltaY * mouseSensitivity);
            animator.SetFloat("GrabHeight", grabHeight);
        }


        if(Input.GetMouseButtonUp(0) && GrabLeft){
            animator.SetBool("Grab",false);
            GrabLeft = false;
        }
        if(Input.GetMouseButtonUp(1) && GrabRight){
            animator.SetBool("Grab",false);
            GrabRight = false;
        }

        
    }

}
