using UnityEngine;

public class PushableChicken : MonoBehaviour
{
    [Header("Settings")]
    public Grid mapGrid;
    [Tooltip("How fast it glides to the center of the cell AFTER you let go")]
    public float snapSpeed = 2.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isSnapping = false;
    private Vector2 targetSnapPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // 1. Is Unity's physics engine moving the chicken? (Are we being pushed?)
        if (rb.linearVelocity.magnitude > 0.2f)
        {
            isSnapping = false; // Cancel snapping if we get pushed again

            // Handle Animations & Sprite Flipping based on actual physical momentum
            if (animator != null) animator.SetBool("isMoving", true);

            if (rb.linearVelocity.x > 0.05f) spriteRenderer.flipX = false;
            else if (rb.linearVelocity.x < -0.05f) spriteRenderer.flipX = true;
        }
        else
        {
            // 2. We have stopped moving. Turn off walk animation.
            if (animator != null) animator.SetBool("isMoving", false);

            // 3. Figure out the nearest grid cell to snap to
            if (!isSnapping)
            {
                Vector3Int nearestCell = mapGrid.WorldToCell(transform.position);
                Vector3 center3D = mapGrid.GetCellCenterWorld(nearestCell);
                targetSnapPosition = new Vector2(center3D.x, center3D.y);

                // If we aren't already dead-center, start snapping
                if (Vector2.Distance(transform.position, targetSnapPosition) > 0.001f)
                {
                    isSnapping = true;
                }
            }

            // 4. Smoothly glide the rest of the way to the center
            if (isSnapping)
            {
                Vector2 newPos = Vector2.MoveTowards(rb.position, targetSnapPosition, snapSpeed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);

                // Lock into place once we reach the center
                if (Vector2.Distance(rb.position, targetSnapPosition) < 0.001f)
                {
                    rb.position = targetSnapPosition;
                    isSnapping = false;
                }
            }
        }
    }
}