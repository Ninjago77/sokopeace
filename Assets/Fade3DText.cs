using System.Collections;
using UnityEngine;
using TMPro;

public class Fade3DText : MonoBehaviour
{
    private TextMeshPro textMesh;
    public float duration = 1.0f; // 1 second total fade time



    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        // Start the 1-second fade immediately
        StartCoroutine(FadeOutText());
    }

    private IEnumerator FadeOutText()
    {
        // Force the text to be completely opaque at the start
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1f);

        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;

            // Calculate new alpha value decreasing from 1 to 0
            float alpha = Mathf.Lerp(1f, 0f, currentTime / duration);

            // Apply alpha directly to the text vertex colors
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);

            yield return null;
        }

        // Ensure it is completely invisible at the end
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0f);
    }
}
