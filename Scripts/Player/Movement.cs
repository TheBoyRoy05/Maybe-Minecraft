using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour{
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -10f;
    Vector3 vertVelocity;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float checkSphereRadius = 0.4f;
    public LayerMask groundMask;
    bool grounded;

    void Update(){
        grounded = Physics.CheckSphere(groundCheck.position, checkSphereRadius, groundMask);
        if(grounded && vertVelocity.y < 0){
            vertVelocity.y = 0f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && grounded){
            vertVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        vertVelocity.y += gravity * Time.deltaTime;
        controller.Move(vertVelocity * Time.deltaTime);
    }
}
