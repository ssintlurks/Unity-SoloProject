using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Camera playerCam;
    private Rigidbody rb;
    Ray jumpRay;
    Ray interactRay;
    RaycastHit interactHit;
    GameObject pickupObj;

    public PlayerInput input;
    public Transform weaponSlot;
    public Weapon currentWeapon;

    float inputX;
    float inputY;

    public int health = 3;
    public int maxHealth = 5;

    public float speed = 5f;
    public float jumpHeight = 2.5f;
    public float groundDetectionDistance = 1.1f;
    public float interactDistance = 1f;

    public bool attacking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = GetComponent<PlayerInput>();
        interactRay = new Ray(transform.position, transform.forward);
        jumpRay = new Ray();
        rb = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        weaponSlot = playerCam.transform.GetChild(0);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Camera Handler
        Quaternion playerRotation = playerCam.transform.rotation;
        playerRotation.x = 0;
        playerRotation.z = 0;
        transform.rotation = playerRotation;

        // Jump Ray setting
        jumpRay.origin = transform.position;
        jumpRay.direction = -transform.up;

        interactRay.origin = playerCam.transform.position;
        interactRay.direction = playerCam.transform.forward;

        if (Physics.Raycast(interactRay, out interactHit, interactDistance))
        {
            if (interactHit.collider.tag == "weapon")
            {
                pickupObj = interactHit.collider.gameObject;
            }
        }
        else
            pickupObj = null;
        if (currentWeapon)
            if (currentWeapon.holdtoAttack && attacking
                currentWeapon.fire();

        // Movement System
        Vector3 tempMove = rb.linearVelocity;

        tempMove.x = inputY * speed;
        tempMove.z = inputX * speed;

        rb.linearVelocity = (tempMove.x * transform.forward) +
            (tempMove.y * transform.up) + 
            (tempMove.z * transform.right);
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (currentWeapon)
        {
            if (currentWeapon.holdToAttack)
            {
                if (context.ReadValueAsButton())
                    attacking = true;
                else
                    attacking = false;
            }
            else if (context.ReadValueAsButton())
                currentWeapon.fire();
        }


    }

    public void Reload()
    {
        if (currentWeapon)
            currentWeapon.reload();
    }

    public void Interact()
    {
        if (pickupObj)
        {
            if (pickupObj.tag == "weapon")
                pickupObj.GetComponent<Weapon>().equip(this);
        }
        else
            Reload();
    }

    public void DropWeapon()
    {
        if (currentWeapon)
        {
            currentWeapon.GetComponent<Weapon>().unequip();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 InputAxis = context.ReadValue<Vector2>();

        inputX = InputAxis.x;
        inputY = InputAxis.y;

    }


    public void Jump()
    {
        if (Physics.Raycast(jumpRay, groundDetectionDistance))
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "killzone")
            health = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "health" && health < maxHealth)
        {
            health++;
            Destroy(collision.gameObject);
        }
    }
}
 