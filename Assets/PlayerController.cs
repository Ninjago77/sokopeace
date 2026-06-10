using UnityEngine;
using System.Collections.Generic; // This MUST be at the top for lists to work!

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private Grid mapGrid; // Drag your Grid object here in the Inspector

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;

    private bool isSnapping = false;
    private Vector2 targetSnapPosition;

    // A list to track the order of key presses
    private List<Vector2> inputHistory = new List<Vector2>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // 1. Detect KEY DOWN events (Add to input history)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) AddInput(Vector2.up);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) AddInput(Vector2.down);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) AddInput(Vector2.left);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) AddInput(Vector2.right);

        // 2. Detect KEY UP events (Remove from input history)
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)) RemoveInput(Vector2.up);
        if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow)) RemoveInput(Vector2.down);
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow)) RemoveInput(Vector2.left);
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow)) RemoveInput(Vector2.right);

        // 3. Process the latest input
        if (inputHistory.Count > 0)
        {
            // Get the last item in the list (the most recently pressed key)
            moveInput = inputHistory[inputHistory.Count - 1];
        }
        else
        {
            moveInput = Vector2.zero;
        }

        // 4. Movement state & snapping
        if (moveInput.magnitude > 0)
        {
            isSnapping = false; // Interrupted snapping if we start walking again

            animator.SetFloat("MoveX", moveInput.x);
            animator.SetFloat("MoveY", moveInput.y);
        }
        else if (!isSnapping)
        {
            // No inputs active -> Snap to nearest grid center
            Vector3Int currentCell = mapGrid.WorldToCell(transform.position);
            targetSnapPosition = mapGrid.GetCellCenterWorld(currentCell);
            isSnapping = true;
        }
    }

    private void AddInput(Vector2 direction)
    {
        if (!inputHistory.Contains(direction))
        {
            inputHistory.Add(direction);
        }
    }

    private void RemoveInput(Vector2 direction)
    {
        // Safety check: Don't remove the direction if the player is still holding a duplicate key 
        // (e.g. if they let go of 'W' but are still holding the 'Up Arrow' key)
        if (direction == Vector2.up && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))) return;
        if (direction == Vector2.down && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))) return;
        if (direction == Vector2.left && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))) return;
        if (direction == Vector2.right && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))) return;

        if (inputHistory.Contains(direction))
        {
            inputHistory.Remove(direction);
        }
    }

    void FixedUpdate()
    {
        if (!isSnapping)
        {
            // Walking
            rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Gliding to grid center
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetSnapPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            if (Vector2.Distance(rb.position, targetSnapPosition) < 0.01f)
            {
                rb.position = targetSnapPosition;
                isSnapping = false;

                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveY", 0);
            }
        }
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }
}