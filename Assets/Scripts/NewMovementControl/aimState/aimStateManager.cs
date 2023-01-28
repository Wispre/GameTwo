using UnityEngine;
using Cinemachine;

public class aimStateManager : MonoBehaviour
{
    aimBaseState currentState;
    public idleFireState idleFire = new idleFireState();
    public aimState Aim = new aimState();

    public float xAxis,yAxis;

    public Animator anim;
    public Transform camFollowPos;
    float mouseSense =1;

    public CinemachineVirtualCamera vCam;
    public float aimFov =40;
    public float idleFov;
    public float currentFov;
    public float fovSmoothSpeed=10;
    

    private void Start() {
        vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        idleFov = vCam.m_Lens.FieldOfView;
        anim = GetComponentInChildren<Animator>();
        SwitchState(idleFire);
    }
    void Update()
    {
        xAxis += Input.GetAxisRaw("Mouse X")*mouseSense;
        yAxis += Input.GetAxisRaw("Mouse Y")*mouseSense;
        yAxis = Mathf.Clamp(yAxis,-80,80);

        vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView,currentFov,fovSmoothSpeed*Time.deltaTime);

        currentState.UpdateState(this);

    }

    private void LateUpdate() {
        camFollowPos.localEulerAngles = new Vector3(yAxis,camFollowPos.localEulerAngles.y,camFollowPos.localEulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis,transform.eulerAngles.z);
    }

    public void SwitchState(aimBaseState state){
        currentState = state;
        currentState.EnterState(this);
    }
}
