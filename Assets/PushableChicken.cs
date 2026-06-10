using UnityEngine;
using System.Collections;

public class PushableChicken : MonoBehaviour
{
    [Header("Settings")]
    public float gridSize = 0.16f;
    public float moveSpeed = 2.5f;

    private bool isMoving = false;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer; // Added to handle left/right flipping

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Grab the Sprite Renderer
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isMoving) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null && player.GetMoveInput().magnitude > 0)
            {
                TryPush(player.GetMoveInput());
            }
        }
    }

    private void TryPush(Vector2 direction)
    {
        Vector2 targetPos = rb.position + (direction * gridSize);

        Vector2 checkSize = new Vector2(gridSize * 0.9f, gridSize * 0.9f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(targetPos, checkSize, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == this.gameObject || hit.CompareTag("Player")) continue;
            if (hit.isTrigger) continue;

            return; // Blocked!
        }

        // Path is clear! Start slide and pass the push direction to the routine
        StartCoroutine(MoveRoutine(targetPos, direction));
    }

    private IEnumerator MoveRoutine(Vector2 targetPos, Vector2 direction)
    {
        isMoving = true;

        // 1. Handle Sprite Flipping (Only updates on horizontal movement)
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false; // Face Right (default art direction)
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = true; // Flip to face Left
        }
        // If direction.x is 0 (moving Up/Down), we do nothing, preserving the last faced direction!

        // 2. Start walking animation
        if (animator != null)
        {
            animator.SetBool("isMoving", true);
        }

        // Smoothly glide to the target
        while (Vector2.Distance(rb.position, targetPos) > 0.005f)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        rb.position = targetPos;
        isMoving = false;

        // 3. Stop walking animation (returns to idle)
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }
    }
}