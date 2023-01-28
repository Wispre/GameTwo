using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class idleFireState : aimBaseState
{
    public override void EnterState (aimStateManager aim){
        aim.anim.SetBool("Aiming",false);
        aim.currentFov = aim.idleFov;
    }
    public override void UpdateState (aimStateManager aim){
        if(Input.GetKey(KeyCode.Mouse1)){
            aim.SwitchState(aim.Aim);
        }
    }
}
