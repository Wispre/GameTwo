using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aimState : aimBaseState
{
    public override void EnterState (aimStateManager aim){
        aim.anim.SetBool("Aiming",true);
        aim.currentFov = aim.aimFov;
        Debug.Log(aim.currentFov);
    }
    public override void UpdateState (aimStateManager aim){
        if(Input.GetKeyUp(KeyCode.Mouse1)){
            aim.SwitchState(aim.idleFire);
        }
    }
}
