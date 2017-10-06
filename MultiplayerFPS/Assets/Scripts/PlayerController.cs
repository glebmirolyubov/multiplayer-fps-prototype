using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensetivity = 3f;

    [SerializeField]
    private float thrusterForce = 1000f;

    [Header("Spring settings:")]
    //[SerializeField]
    //private JointDriveMode mode;
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    //Component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    private void Update()
    {
        //Calculate our movement velocity as 3D Vector
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        // Final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

        //Animate movement
        animator.SetFloat("ForwardVelocity", _zMov);

        // Apply movement
        motor.Move(_velocity);

        //Calculate rotation as 3D Vector (turning around)
        float _yRot = Input.GetAxisRaw("Mouse X");
        Vector3 rotation = new Vector3(0f, _yRot, 0f) * lookSensetivity;

        //Apply rotation
        motor.Rotate(rotation);

		//Calculate rotation as 3D Vector (turning around)
		float _xRot = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = _xRot * lookSensetivity;

		//Apply rotation
		motor.RotateCamera(cameraRotationX);

        Vector3 _thrusterForce = Vector3.zero;
        if (Input.GetButton("Jump")) {
            _thrusterForce = Vector3.up * thrusterForce;
            SetJointSettings(0f);
        } else {
            SetJointSettings(jointSpring);
        }

        //Apply thruster force
        motor.ApplyThruster(_thrusterForce);

	}

    private void SetJointSettings (float _jointSpring) {
        joint.yDrive = new JointDrive
        {
            //mode = JointDriveMode,
            positionSpring = jointSpring,
            maximumForce = jointMaxForce
        };
    }

}
