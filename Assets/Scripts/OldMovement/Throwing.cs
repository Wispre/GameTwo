using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwing : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    [Header("Settings")]
    public int Ammo;
    public float throwColdDown;

    [Header("Throw")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;


    private void Start(){
        readyToThrow = true;
    }


    private void Update() {
        //Check 1. userClick, 2.readyToThrow, 3.Ammo
        if(Input.GetKeyDown(throwKey)&&readyToThrow&&Ammo>0){
            //call throw
            Throw();
        }
    }
    private void Throw(){
        readyToThrow = false;
        //Instantiate object to Throw
        GameObject projectile = Instantiate(objectToThrow,attackPoint.position,cam.rotation);
        //get Rigidbody from the projectile
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        //Calculate throw direction
        Vector3 throwDirection = cam.transform.forward;

        RaycastHit hit;
        //Default range check 500
        if(Physics.Raycast(cam.position,cam.forward,out hit,500f)){
            throwDirection = (hit.point-attackPoint.position).normalized;
        }

        //Add force in the direction you looking
        Vector3 addForce = throwDirection* throwForce+transform.up*throwUpwardForce;
        projectileRb.AddForce(addForce,ForceMode.Impulse);
        //decrement ammo
        Ammo--;

        //throw coldDown
        Invoke(nameof(ResetThrow), throwColdDown);

    }

    private void ResetThrow(){
        readyToThrow = true;

    }
}
