using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector3 cameraOffset = new Vector3(0,.5f,.2f);
    Vector2 cameraRotation = Vector2.zero;
    Camera playerCam;
    InputAction lookAxis;
    private Rigidbody rb;

    float inputX;
    float inputY;

    public float Xsensitivity = .1f;
    public float Ysensitivity = .1f;
    public float speed = 5f;
    public float camRotationLimit = 90;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        lookAxis = GetComponent<PlayerInput>().currentActionMap.FindAction("Look");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Camera Handler
        playerCam.transform.position = transform.position + cameraOffset;

        cameraRotation.x += lookAxis.ReadValue<Vector2>().x * Xsensitivity;
        cameraRotation.y += lookAxis.ReadValue<Vector2>().y * Ysensitivity;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y, -camRotationLimit, camRotationLimit);
        playerCam.transform.rotation = Quaternion.Euler(-cameraRotation.y, cameraRotation.x, 0);
        transform.rotation = Quaternion.AngleAxis(cameraRotation.x, Vector3.up);

        // Movement System
        Vector3 tempMove = rb.linearVelocity;

        tempMove.x = inputY * speed;
        tempMove.z = inputX * speed;

        rb.linearVelocity = (tempMove.x * transform.forward) +
            (tempMove.y * transform.up) + 
            (tempMove.z * transform.right);
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 InputAxis = context.ReadValue<Vector2>();

        inputX = InputAxis.x;
        inputY = InputAxis.y;

    }



}
 