using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 5f;

    [SerializeField]
    private float thrustedFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;
    [SerializeField]
    private float thrustedForce = 1000f;

    public float GetThrusterFuelAmount() {
        return thrusterFuelAmount;
    }

    [SerializeField]
    private LayerMask enviromentMask = 0;


    [Header("Spring settings:")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;


    //Component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();  
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);      
    }

    // Update is called once per frame
    void Update()
    {
        //Setting targer position for spring
        //This makes the physics act rgith when it comes to applying gravity when flying over objects
        RaycastHit _hit;
        if(Physics.Raycast(transform.position, Vector3.down, out _hit, 100f, enviromentMask)){
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        } else {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }

        //Calcu;ate ,pve,emt ve;pcotu as a 3d vector
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 _movHoricontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        //Final movement vector
        Vector3 _velocity = (_movHoricontal + _movVertical) * speed;

        //Animate movement
        animator.SetFloat("ForwardVelocity", _zMov);

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
        if(Input.GetButton("Jump") && thrusterFuelAmount > 0f) {
            thrusterFuelAmount -= thrustedFuelBurnSpeed * Time.deltaTime;
            if(thrusterFuelAmount >= 0.01f) {
                _thrusterForce = Vector3.up * thrustedForce;
                SetJointSettings(0f);
            }
        } else {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        //Apply the thruster force
        motor.ApplyThruster(_thrusterForce);
    }

    private void SetJointSettings(float _jointSpring) {
        joint.yDrive = new JointDrive{ positionSpring = _jointSpring, maximumForce = jointMaxForce };
    }
}
