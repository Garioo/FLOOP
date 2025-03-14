using UnityEngine;
using System.Collections;

public class FloatingLeaf : MonoBehaviour
{
    public GameObject leafPrefab; // Prefab to instantiate
    public Transform targetPosition; // The empty GameObject defining direction
    public float speed = 1f; // Base speed
    public float driftAmount = 0.5f; // How much the leaf drifts
    public float rotationSpeed = 30f; // Random rotation speed
    public float lifeTime = 5f; // Total lifetime
    public Vector2 randomSizeRange = new Vector2(0.5f, 1.5f); // Min and Max scale

    private SpriteRenderer spriteRenderer;
    private float fadeDuration = 1f;

    void Start()
    {
        GameObject leaf = Instantiate(leafPrefab, transform.position, Quaternion.identity);
        leaf.transform.localScale = Vector3.one * Random.Range(randomSizeRange.x, randomSizeRange.y);
        spriteRenderer = leaf.GetComponent<SpriteRenderer>();
        StartCoroutine(FadeIn());
        StartCoroutine(LeafMovement(leaf));
    }

    IEnumerator LeafMovement(GameObject leaf)
    {
        Vector3 startPosition = leaf.transform.position;
        Vector3 endPosition = targetPosition.position;
        float elapsedTime = 0f;

        float randomRotationSpeed = Random.Range(-rotationSpeed, rotationSpeed);
        float randomDrift = Random.Range(-driftAmount, driftAmount);

        while (elapsedTime < lifeTime)
        {
            float t = elapsedTime / lifeTime;
            leaf.transform.position = Vector3.Lerp(startPosition, endPosition, t) + new Vector3(randomDrift * Mathf.Sin(Time.time * 2f), 0, 0);
            leaf.transform.Rotate(Vector3.forward * randomRotationSpeed * Time.deltaTime);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(FadeOut(leaf));
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;

        while (elapsedTime < fadeDuration)
        {
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            spriteRenderer.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeOut(GameObject leaf)
    {
        float elapsedTime = 0f;
        Color color = spriteRenderer.color;

        while (elapsedTime < fadeDuration)
        {
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(leaf); // Remove leaf after fade out
    }
}


