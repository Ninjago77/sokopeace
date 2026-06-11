using UnityEngine;

public class PushableChicken : MonoBehaviour
{
    [Header("Settings")]
    private Grid mapGrid;
    public float snapSpeed = 2.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isSnapping = false;
    private Vector2 targetSnapPosition;
    private bool isBeingPushed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mapGrid = GameObject.FindGameObjectWithTag("MapGrid")?.GetComponent<Grid>();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                // Yield to the player if they are walking OR if they are snapping into our cell
                if (player.GetMoveInput().magnitude > 0 || player.IsSnapping())
                {
                    isBeingPushed = true;
                    isSnapping = false;
                }
                else
                {
                    isBeingPushed = false;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isBeingPushed = false;
        }
    }

    void FixedUpdate()
    {
        if (isBeingPushed)
        {
            // Physics handles the push, we just play animations
            if (animator != null) animator.SetBool("isMoving", true);

            if (rb.linearVelocity.x > 0.05f) spriteRenderer.flipX = false;
            else if (rb.linearVelocity.x < -0.05f) spriteRenderer.flipX = true;
        }
        else
        {
            if (animator != null) animator.SetBool("isMoving", false);

            if (!isSnapping)
            {
                // Find nearest cell
                Vector3Int nearestCell = mapGrid.WorldToCell(rb.position);
                Vector3 center3D = mapGrid.GetCellCenterWorld(nearestCell);
                targetSnapPosition = new Vector2(center3D.x, center3D.y);

                if (Vector2.Distance(rb.position, targetSnapPosition) > 0.005f)
                {
                    isSnapping = true;
                }
            }

            if (isSnapping)
            {
                // EXACT VELOCITY SNAPPING (No more physics fighting!)
                Vector2 diff = targetSnapPosition - rb.position;
                float distance = diff.magnitude;

                if (distance > 0.005f)
                {
                    float speedNeeded = distance / Time.fixedDeltaTime;
                    float currentSpeed = Mathf.Min(speedNeeded, snapSpeed);
                    rb.linearVelocity = diff.normalized * currentSpeed;
                }
                else
                {
                    // Reached Center
                    rb.position = targetSnapPosition;
                    rb.linearVelocity = Vector2.zero;
                    isSnapping = false;
                }
            }
        }
    }
}