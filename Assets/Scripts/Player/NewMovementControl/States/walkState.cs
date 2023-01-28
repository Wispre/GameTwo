using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walkState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Walking",true);
    }
    public override void UpdateState(MovementStateManager movement)
    {
        if(Input.GetKey(KeyCode.LeftShift)){
            //transition from walking to run state.
            ExitState(movement,movement.Run);
        }else if(Input.GetKeyDown(KeyCode.LeftControl)){
            //transition from walking to Crouch state.
            ExitState(movement,movement.Crouch);
        }else if(movement.dir.magnitude<0.1f){
            //transition from walking to idle state.
            ExitState(movement,movement.Idle);
        }
        if(movement.verticalInput <0){
            movement.currentMoveSpeed = movement.walkBackSpeed;
        }else{
            movement.currentMoveSpeed = movement.walkSpeed;
        }
    }

    
    void ExitState(MovementStateManager movement, MovementBaseState goTostate){
        movement.anim.SetBool("Walking",false);
        movement.SwitchState(goTostate);
    }
}

