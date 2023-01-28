using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{


    [Header("Movement")]
    public float currentMoveSpeed;
    public float walkSpeed=3, walkBackSpeed=2;
    public float runSpeed=7, runBackSpeed=5;
    public float crouchSpeed=2, crouchBackSpeed=1;
    MovementBaseState currentState;
    public idleState Idle = new idleState();
    public walkState Walk = new walkState();
    public crouchState Crouch = new crouchState();
    public runState Run = new runState();

    public Animator anim;
    public Vector3 dir;
    public float horizontalnput, verticalInput;
    CharacterController controller;

    public float groundYOffset;
    public float gravity = -9.8f;
    Vector3 velocity;
    public LayerMask groundMask;
    Vector3 spherePos;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        SwitchState(Idle);
    }

    // Update is called once per frame
    void Update()
    {
        getDirectionMove();
        Gravity();

        //update animation
        anim.SetFloat("hzInput",horizontalnput);
        anim.SetFloat("vInput",verticalInput);

        currentState.UpdateState(this);
    }

    public void SwitchState(MovementBaseState state){
        currentState = state;
        currentState.EnterState(this);
    }
    void getDirectionMove(){
        horizontalnput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        dir = transform.forward*verticalInput+transform.right*horizontalnput;

        controller.Move(Vector3.ClampMagnitude(dir, 1.0f)*currentMoveSpeed*Time.deltaTime);
    }

    bool isGrounded(){
        spherePos = new Vector3(transform.position.x,transform.position.y,transform.position.z);
        if(Physics.CheckSphere(spherePos,controller.radius-0.05f,groundMask)){
            return true;
        }else{
            return false;
        }
    }

    void Gravity(){
        if(!isGrounded()){
            velocity.y+= gravity*Time.deltaTime;
        }else if(velocity.y<0){
            velocity.y=-2;
        }
        controller.Move(velocity*Time.deltaTime);
    }
}
