using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private Vector3 thrusterForce = Vector3.zero;

    [SerializeField]
    private float cameraRoatationLimit = 85f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Gets movement vector
    public void Move(Vector3 _velocity) {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _rotation) {
        rotation = _rotation;
    }
    
    public void RotateCamera(float _cameraRotationX) {
        cameraRotationX = _cameraRotationX;
    }

    public void ApplyThruster (Vector3 _thrusterForce) {
        thrusterForce = _thrusterForce;
    }

    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    // Perform movement based on velocity variable
    void PerformMovement() {
        if (velocity != Vector3.zero) {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if (thrusterForce != Vector3.zero) {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    void PerformRotation() {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null) {
            //cam.transform.Rotate(-cameraRotation);

            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRoatationLimit, cameraRoatationLimit);

            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }

}
