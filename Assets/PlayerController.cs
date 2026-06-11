using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.5f;
    private Grid mapGrid;
    public bool CanMove { get; set; } = false;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;

    private bool isSnapping = false;
    private Vector2 targetSnapPosition;

    private List<Vector2> inputHistory = new List<Vector2>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        mapGrid = GameObject.FindGameObjectWithTag("MapGrid")?.GetComponent<Grid>();

        Invoke("SET_CAN_MOVE_TRUE", 0.5f);
    }

    void SET_CAN_MOVE_TRUE()
    {
        CanMove = true;
    }

    void Update()
    {
        if (!CanMove) return;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) AddInput(Vector2.up);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) AddInput(Vector2.down);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) AddInput(Vector2.left);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) AddInput(Vector2.right);

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)) RemoveInput(Vector2.up);
        if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow)) RemoveInput(Vector2.down);
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow)) RemoveInput(Vector2.left);
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow)) RemoveInput(Vector2.right);

        if (inputHistory.Count > 0)
            moveInput = inputHistory[inputHistory.Count - 1];
        else
            moveInput = Vector2.zero;

        if (moveInput.magnitude > 0)
        {
            isSnapping = false;
            animator.SetFloat("MoveX", moveInput.x);
            animator.SetFloat("MoveY", moveInput.y);
        }
        else if (!isSnapping)
        {
            Vector3Int currentCell = mapGrid.WorldToCell(transform.position);
            targetSnapPosition = mapGrid.GetCellCenterWorld(currentCell);
            isSnapping = true;
        }
    }

    private void AddInput(Vector2 direction)
    {
        if (!inputHistory.Contains(direction)) inputHistory.Add(direction);
    }

    private void RemoveInput(Vector2 direction)
    {
        if (direction == Vector2.up && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))) return;
        if (direction == Vector2.down && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))) return;
        if (direction == Vector2.left && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))) return;
        if (direction == Vector2.right && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))) return;

        if (inputHistory.Contains(direction)) inputHistory.Remove(direction);
    }

    void FixedUpdate()
    {
        if (!isSnapping)
        {
            // Walking normally
            rb.linearVelocity = moveInput * moveSpeed;
        }
        else
        {
            // EXACT VELOCITY SNAPPING (Prevents overshoot & Jitter)
            Vector2 diff = targetSnapPosition - rb.position;
            float distance = diff.magnitude;

            if (distance > 0.005f)
            {
                // Calculate speed needed to reach target exactly this frame
                float speedNeeded = distance / Time.fixedDeltaTime;
                float currentSpeed = Mathf.Min(speedNeeded, moveSpeed); // Clamp so it doesn't move too fast
                rb.linearVelocity = diff.normalized * currentSpeed;
            }
            else
            {
                // Reached center
                rb.position = targetSnapPosition;
                rb.linearVelocity = Vector2.zero;
                isSnapping = false;

                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveY", 0);
            }
        }
    }

    public Vector2 GetMoveInput() { return moveInput; }
    public bool IsSnapping() { return isSnapping; }
}