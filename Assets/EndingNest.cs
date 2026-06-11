using UnityEngine;

public class EndingNest : MonoBehaviour
{
    [Header("Settings")]
    private Grid mapGrid;

    public Sprite nestSprite;
    public Sprite eggNestSprite;
    private SpriteRenderer spriteRenderer;
    public bool isFilled;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        isFilled = false;

    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = nestSprite;
        mapGrid = GameObject.FindGameObjectWithTag("MapGrid")?.GetComponent<Grid>();

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Chicken") && !isFilled)
        {
            isFilled = true;
            spriteRenderer.sprite = eggNestSprite;
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Destroy(collision.gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
