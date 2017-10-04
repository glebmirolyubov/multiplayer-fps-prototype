using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensetivity = 3f;

    private PlayerMotor motor;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    private void Update()
    {
        //Calculate our movement velocity as 3D Vector
        float _xMov = Input.GetAxisRaw("Horizontal");
        float _zMov = Input.GetAxisRaw("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        // Final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical).normalized * speed;

        // Apply movement
        motor.Move(_velocity);

        //Calculate rotation as 3D Vector (turning around)
        float _yRot = Input.GetAxisRaw("Mouse X");
        Vector3 rotation = new Vector3(0f, _yRot, 0f) * lookSensetivity;

        //Apply rotation
        motor.Rotate(rotation);

		//Calculate rotation as 3D Vector (turning around)
		float _xRot = Input.GetAxisRaw("Mouse Y");
		Vector3 cameraRotation = new Vector3(_xRot, 0f, 0f) * lookSensetivity;

		//Apply rotation
		motor.RotateCamera(cameraRotation);
    }

}
