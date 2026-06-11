using UnityEngine;

public class EndingNest : MonoBehaviour
{
    [Header("Settings")]
    private Grid mapGrid;

    public Sprite nestSprite;
    public Sprite eggNestSprite;
    private SpriteRenderer spriteRenderer;
    private bool isFilled;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isFilled = false;
        spriteRenderer.sprite = nestSprite;
        mapGrid = GameObject.FindGameObjectWithTag("MapGrid")?.GetComponent<Grid>();

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Chicken"))
        {
            isFilled = true;
            spriteRenderer.sprite = eggNestSprite;
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
