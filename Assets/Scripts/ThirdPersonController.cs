using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public float speed = 10;
    public float jumpHeight = 10;
    public float airControl = 10;
    public float gravity = 9.81f;
    CharacterController controller;
    Vector3 input;
    Vector3 moveDirection;

    public GameObject startingPosition;
    //Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        //anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        startingPosition = GameObject.FindGameObjectWithTag("startingPos");
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        if (moveX != 0 || moveY != 0)
        {

            //anim.SetInteger("animState", 1);
        }
        else if (moveX == 0 && moveY == 0 && controller.isGrounded)
        {
            //anim.SetInteger("animState", 0);
        }

        //input = new Vector3(moveX, 0, moveY);
        input = (transform.right * moveX + transform.forward * moveY).normalized;
        input *= speed;

        if (input.magnitude > 0.01f)
        {
            float cameraYawRotation = Camera.main.transform.eulerAngles.y;
            Quaternion newRotation = Quaternion.Euler(0f, cameraYawRotation, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * 10);
        }

        if (controller.isGrounded)
        {

            moveDirection = input;

            if (Input.GetButton("Jump"))
            {
                Debug.Log("HERE");
                //anim.SetInteger("animState", 2);
                moveDirection.y = Mathf.Sqrt(2 * jumpHeight * gravity);
            }
            else
            {
                moveDirection.y = 0.0f;
            }

        }
        else
        {
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    public void ResetPosition()
    {
        transform.position = startingPosition.transform.position;
    }
}
