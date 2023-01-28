using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crouchState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Crouching",true);
    }
    public override void UpdateState(MovementStateManager movement)
    {
        if(Input.GetKey(KeyCode.LeftShift)){
            ExitState(movement,movement.Run);
        }
        if(Input.GetKeyDown(KeyCode.LeftControl)){
            if(movement.dir.magnitude<0.1f){
                ExitState(movement,movement.Idle);
            }else{
                ExitState(movement,movement.Walk);
            }
        }
        //update speed
        if(movement.verticalInput <0){
            movement.currentMoveSpeed = movement.crouchBackSpeed;
        }else{
            movement.currentMoveSpeed = movement.crouchSpeed;
        }
    }
    void ExitState(MovementStateManager movement, MovementBaseState goTostate){
        movement.anim.SetBool("Crouching",false);
        movement.SwitchState(goTostate);
    }
}
