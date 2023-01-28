using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float speed;

    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;
    public float slideSpeed;

    private float desiredSpeed;
    private float lastDesiredSpeed;

    
    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpColdown;
    //air
    public float multiplier;

    [Header("Crouch")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Slope")]
    public float maxAngle;
    private RaycastHit slopeHit;
    public bool exitSlope;

    [Header("Input")]
    public KeyCode crouchKey= KeyCode.LeftControl;
    public KeyCode SpritKey = KeyCode.LeftShift;
    public KeyCode SlideKey = KeyCode.LeftControl;

    public KeyCode JumpKey = KeyCode.Space;


    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask ThisIsGround;
    bool grounded;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    bool readyToJump;
    Vector3 moveDirection;
    Rigidbody rb;




    [Header("References")]
    public Transform playerObj;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;

    [Header("Input")]
    private float HorizontalInput;
    public bool sliding;
    public bool crouching;



    public MovementState state;
    //Player movement State
    public enum MovementState{
        idle,
        walk,
        sprint,
        air,
        crouch,
        Sliding
    }

    
    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        //Get the local y scale
        startYScale = transform.localScale.y;
    }

    private void Update() {

        //ground check
        grounded = Physics.Raycast(transform.position,
        Vector3.down,playerHeight*0.5f+0.2f,ThisIsGround);

        horizontalInput =Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        //handle jump
        if(Input.GetKey(JumpKey) && readyToJump && grounded){
            readyToJump = false;
            Jump();
            //Allow to continuous to jump after coldown
            Invoke(nameof(setReadyToJump),jumpColdown);
        }
        
        //start crouch
        if(Input.GetKeyDown(crouchKey) && horizontalInput==0 && verticalInput==0)
        {
            transform.localScale = new Vector3(transform.localScale.x,
            crouchYScale,transform.localScale.z);
            //Add force to prevent floating because of shrink
            rb.AddForce(Vector3.down*5f, ForceMode.Impulse);
            crouching = true;
        }

        //Stop crouch
        if(Input.GetKeyUp(crouchKey)){
            transform.localScale = new Vector3(transform.localScale.x,
            startYScale,transform.localScale.z);
            crouching = false;
        }

        //To Slide you have to have both leftControl and direction
        if(Input.GetKeyDown(SlideKey) && 
        (HorizontalInput!=0 || verticalInput!=0)){
            Debug.Log("Inside Slide");
            Slide();
        }

        if(Input.GetKeyUp(SlideKey)&& sliding){
            //Stop slide
            stopSlide();
        }

        //speedControl
        SpeedControl();
        //setMovementState
        setMovementState();

        //handle drag
        if(grounded){
            rb.drag = groundDrag;
        }else{
            rb.drag =0;
        }
    }

    private void FixedUpdate() {
        if(sliding){
            slidingMovement();
        }
        MovePlayer();
    }


    //Setter for MovementState
    private void setMovementState(){
        //Sliding
        if(sliding){
            state = MovementState.Sliding;

            if(checkSlope() && rb.velocity.y <0.1f){
                desiredSpeed = slideSpeed;
            }else{
                desiredSpeed = sprintSpeed;
            }
        }
        //Crouch
        else if(crouching){
            state = MovementState.crouch;
            desiredSpeed = crouchSpeed;

        }
        //Sprint
        else if(grounded && Input.GetKey(SpritKey)){
            state = MovementState.sprint;
            desiredSpeed = sprintSpeed;
        }
        
        //walking
        else if(grounded){
            //idle
            if(rb.velocity.magnitude ==0){
                state = MovementState.idle;
                desiredSpeed = walkSpeed;
            }else{
                state = MovementState.walk;
                desiredSpeed = walkSpeed;
            }
        }else{
            //Air
            state = MovementState.air;
        }

        //check if desiredSpeed change a lot
        if(Mathf.Abs(desiredSpeed-lastDesiredSpeed)>4f){
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpSpeed());
        }else{
            //small speed difference set directly
            speed = desiredSpeed;
        }
        lastDesiredSpeed =desiredSpeed;
        //turn gravity off onSlope
        // rb.useGravity = !checkSlope();
    }

    //change move speed to desired speed overtime
    private IEnumerator SmoothlyLerpSpeed(){
        //Smooth speed to desired value
        float time =0;
        float difference = Mathf.Abs(desiredSpeed-speed);
        float startValue = speed;

        while(time<difference){
            speed = Mathf.Lerp(startValue, desiredSpeed, time/difference);
            if(checkSlope()){
                float slopeAngle = Vector3.Angle(Vector3.up,slopeHit.normal);
                float slopeAngleIncrease = 1+(slopeAngle/90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }else{
                time += Time.deltaTime * speedIncreaseMultiplier;
            }
            
            yield return null;
        }
        speed = desiredSpeed;
    }


    private void Slide(){
        //set sliding to true
        sliding = true;
        Vector3 playerScale =playerObj.localScale;
        playerObj.localScale = new Vector3(playerScale.x, slideYScale,playerScale.z);

        //apply down force push player to the ground
        rb.AddForce(Vector3.down*5f,ForceMode.Impulse);
        slideTimer = maxSlideTime;
    }

    private void stopSlide(){
        //Set sliding to false
        sliding = false;
        Vector3 playerScale =playerObj.localScale;
        playerObj.localScale = new Vector3(playerScale.x, startYScale,playerScale.z);

    }
    private void slidingMovement(){
        Vector3 inputDirection = orientation.forward*verticalInput + orientation.right*HorizontalInput;
        //if player not on the slope
        if(!checkSlope() ||rb.velocity.y >-0.1f){
            //Add force
            rb.AddForce(inputDirection.normalized*slideForce, ForceMode.Force);
            slideTimer-=Time.deltaTime;
        }else{
            //When player moving down the slope Apply force in the slope movement direction
            rb.AddForce(getSlopeDirection(inputDirection)*slideForce,ForceMode.Force);
        }

        //Slide time count down
        if(slideTimer <= 0){
            //stop the slide
            stopSlide();
        }
    }

    private void MovePlayer(){
        //calculate movement
        moveDirection = orientation.forward * verticalInput + orientation.right*horizontalInput;

        //slope
        if(checkSlope() && !exitSlope){
            rb.AddForce(getSlopeDirection(moveDirection)*speed*20f, ForceMode.Force);
            //prevent Slope jumping,
            if(rb.velocity.y>0){
                rb.AddForce(Vector3.down*80f,ForceMode.Force);
            }
        }
        //onGround
        if(grounded){
            rb.AddForce(moveDirection.normalized*speed*10f,ForceMode.Force);
        }
        else if(!grounded){
            //In air
            rb.AddForce(moveDirection.normalized*speed*10f*multiplier,ForceMode.Force);
        }
        
    }
    
    private void SpeedControl(){
        //Limit speed on Slope
        if(checkSlope() && !exitSlope){
            if(rb.velocity.magnitude > speed){
                rb.velocity = rb.velocity.normalized*speed;
            }
        }else{
            //limit speed on ground
            Vector3 velocityGround = new Vector3(rb.velocity.x,0f,rb.velocity.z);
            if(velocityGround.magnitude > speed){
                Vector3 limitVelocity = velocityGround.normalized*speed;
                rb.velocity = new Vector3(limitVelocity.x,rb.velocity.y,
                limitVelocity.z);
            }
        }
    }
    private void Jump(){
        exitSlope = true;
        //reset y velocity so you jumap at exact same height
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    private void setReadyToJump(){
        readyToJump = true;
        exitSlope = false;
    }

    private bool checkSlope(){
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight*0.5f+0.3f)){
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxAngle && angle!=0;
        }
        return false;
    }

    private Vector3 getSlopeDirection(Vector3 direction){
        return Vector3.ProjectOnPlane(direction,slopeHit.normal).normalized;
    }
}
