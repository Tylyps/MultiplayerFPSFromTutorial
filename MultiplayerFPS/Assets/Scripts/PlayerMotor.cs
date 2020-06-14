﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{

    [SerializeField]
    private Camera cam;

    private Vector3 thrusterForce = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;

    [SerializeField]
    private float cameraRotationLimit = 85f;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Gets a movement vector
    public void Move(Vector3 _velocity){
        velocity = _velocity;
    }
    
    // Gets a rotational vector
    public void Rotate(Vector3 _roation){
        rotation = _roation;
    }

    public void RotateCamera(float _cameraRotationX){
        cameraRotationX = _cameraRotationX;
    }

    //Get a force vector for our thrusters
    public void ApplyThruster(Vector3 _thrusterForce){
        thrusterForce = _thrusterForce;
    }

    void FixedUpdate() {
        PerformMovement();
        PerformRotation();
    }

    void PerformMovement() {
        if(velocity != Vector3.zero) {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if(thrusterForce != Vector3.zero){
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    void PerformRotation() {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if(cam != null){
            // cam.transform.Rotate(-cameraRotationX);
            //Set our rotation and clamp it
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            //Apply our rotation to the transform of our camera
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }
}
