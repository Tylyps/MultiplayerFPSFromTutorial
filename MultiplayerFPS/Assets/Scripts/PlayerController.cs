using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 5f;
    [SerializeField]
    private float thrustedForce = 1000f;

    [Header("Spring settings:")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    private PlayerMotor motor;
    private ConfigurableJoint joint;
    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();  

        SetJointSettings(jointSpring);      
    }

    // Update is called once per frame
    void Update()
    {
        //Calcu;ate ,pve,emt ve;pcotu as a 3d vector
        float _xMov = Input.GetAxisRaw("Horizontal");
        float _zMov = Input.GetAxisRaw("Vertical");

        Vector3 _movHoricontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        //Final movement vector
        Vector3 _velocity = (_movHoricontal + _movVertical).normalized * speed;

        //Apply movment
        motor.Move(_velocity);

        //Calculate rotation as a 3D vector (turning around)
        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        //Apply rotation
        motor.Rotate(_rotation);
        

        //Calculate camera rotation as a 3D vector (turning around)
        float _xRot = Input.GetAxisRaw("Mouse Y");

        float _cameraRotationX =_xRot * lookSensitivity;

        //Apply rotation
        motor.RotateCamera(_cameraRotationX);

        //Calculate the thruster force by player input
        Vector3 _thrusterForce = Vector3.zero;
        if(Input.GetButton("Jump")) {
            _thrusterForce = Vector3.up * thrustedForce;
            SetJointSettings(0f);
        } else {
            SetJointSettings(jointSpring);
        }

        //Apply the thruster force
        motor.ApplyThruster(_thrusterForce);
    }

    private void SetJointSettings(float _jointSpring) {
        joint.yDrive = new JointDrive{ positionSpring = _jointSpring, maximumForce = jointMaxForce };
    }
}
