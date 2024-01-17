using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementScript : MonoBehaviour
{
    private Rigidbody rb;
    private Transform cameraMain;
    private Vector3 cameraPosition;

    public float currentSpeed;
    public float jumpForce = 500;
    private Vector3 slowdownV;
    private Vector2 horizontalMovement;

    public float maxSpeed = 5;
    public float deaccelerationSpeed = 15.0f;
    public float accelerationSpeed = 50000.0f;
    public bool grounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraMain = transform.Find("Main Camera").transform;
    }

    private void FixedUpdate()
    {
        PlayerMovementLogic();
    }

    private void PlayerMovementLogic()
    {
        currentSpeed = rb.velocity.magnitude;
        horizontalMovement = new Vector2(rb.velocity.x, rb.velocity.z);

        if (horizontalMovement.magnitude > maxSpeed)
        {
            horizontalMovement = horizontalMovement.normalized;
            horizontalMovement *= maxSpeed;
        }

        rb.velocity = new Vector3(
            horizontalMovement.x,
            rb.velocity.y,
            horizontalMovement.y
        );

        if (grounded)
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity,
                new Vector3(0, rb.velocity.y, 0),
                ref slowdownV,
                deaccelerationSpeed);
        }

        if (grounded)
        {
            rb.AddRelativeForce(Input.GetAxis("Horizontal") * accelerationSpeed * Time.deltaTime, 0,
                Input.GetAxis("Vertical") * accelerationSpeed * Time.deltaTime);
        }
        else
        {
            rb.AddRelativeForce(Input.GetAxis("Horizontal") * accelerationSpeed / 2 * Time.deltaTime, 0,
                Input.GetAxis("Vertical") * accelerationSpeed / 2 * Time.deltaTime);
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            deaccelerationSpeed = 0.5f;
        }
        else
        {
            deaccelerationSpeed = 0.1f;
        }
    }

    private void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddRelativeForce(Vector3.up * jumpForce);
        }
    }

    private void Update()
    {
        Jumping();
        Crouching();
        WalkingSound();
    }

    private void WalkingSound()
    {
        // Implement walking sound logic here
    }

    private void Crouching()
    {
        if (Input.GetKey(KeyCode.C))
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 0.6f, 1), Time.deltaTime * 15);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * 15);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            if (Vector2.Angle(contact.normal, Vector3.up) < 60)
            {
                grounded = true;
            }
        }
    }

    private void OnCollisionExit()
    {
        grounded = false;
    }
}
