using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float forwardSpeed = 50f;
    public float horizontalSpeed = 10f;
    public float jumpForce = 25f;
    public float extraGravity = 40f; // Extra gravity for tweaking jump dynamics

    private bool canJump = false;
    private bool canSlide = false;

    private float boundaryLeft = -1.6f;
    private float boundaryRight = 1.6f;

    private Rigidbody rb;
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private int jumpID;
    private int slideID;
    private Vector3 targetPosition;
    private bool isGrounded;
    private bool inAir;

    public float maxSpeed = 7;

    public ACTIVE_LANE currentLane;

    public enum ACTIVE_LANE
    {
        LEFT,
        MIDDLE,
        RIGHT
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        jumpID = Animator.StringToHash("Jump");
        slideID = Animator.StringToHash("Slide");
        targetPosition = transform.position;
        currentLane = ACTIVE_LANE.MIDDLE;
        SetSpeed(0);
        SetPlayerActionsState(false);
    }

    public void SetSpeed(float speed)
    {
        forwardSpeed = speed;
    }

    void Update()
    {
        if (forwardSpeed != 0)
        {
            // Forward movement
            Vector3 forwardMovement = forwardSpeed * Time.deltaTime * Vector3.forward;
            rb.MovePosition(rb.position + forwardMovement);

            // Move left
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SwitchLane(false);
            }

            // Move right
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SwitchLane(true);
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded && canJump)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                isGrounded = false;
                inAir = true;
                animator.SetBool(jumpID, true); // Set the jump animation
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) && canSlide)
            {
                canSlide = false;
                animator.SetBool(slideID, true); // Start the slide animation
                SetColliderValues(true);
                Invoke(nameof(StopSlide), 1.0f); // Stop the slide animation after 1 second
            }

            // Horizontal movement
            Vector3 newPosition = new Vector3(targetPosition.x, rb.position.y, rb.position.z);
            rb.MovePosition(Vector3.Lerp(rb.position, newPosition, horizontalSpeed * Time.deltaTime));
        }
    }

    private void FixedUpdate()
    {
        if (inAir)
        {
            Vector3 vel = rb.velocity;
            vel.y -= extraGravity * Time.deltaTime;
            rb.velocity = vel;
        }
    }

    private void SwitchLane(bool moveRight)
    {
        switch (currentLane)
        {
            case ACTIVE_LANE.LEFT:
                if (moveRight)
                {
                    currentLane = ACTIVE_LANE.MIDDLE;
                    targetPosition.x = 0;
                }
                break;

            case ACTIVE_LANE.MIDDLE:
                if (moveRight)
                {
                    currentLane = ACTIVE_LANE.RIGHT;
                    targetPosition.x = boundaryRight;
                }
                else
                {
                    currentLane = ACTIVE_LANE.LEFT;
                    targetPosition.x = boundaryLeft;
                }
                break;

            case ACTIVE_LANE.RIGHT:
                if (!moveRight)
                {
                    currentLane = ACTIVE_LANE.MIDDLE;
                    targetPosition.x = 0;
                }
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player is grounded
        if (collision.contacts[0].point.y <= transform.position.y + 0.1f)
        {
            isGrounded = true;
            inAir = false;
            animator.SetBool(jumpID, false); // Reset the jump animation
        }
    }

    private void StopSlide()
    {
        animator.SetBool(slideID, false); // Stop the slide animation
        canSlide = true; // Allow sliding again
        SetColliderValues(false);
    }

    public void SetPlayerActionsState(bool state)
    {
        canJump = state;
        canSlide = state;
    }

    public void SetColliderValues(bool isSliding)
    {
        capsuleCollider.center = isSliding ? new Vector3(0, 0.35f, 0) : new Vector3(0, 0.5f, 0);
        capsuleCollider.height = isSliding ? 0.8f : 1.7f;
    }
}
