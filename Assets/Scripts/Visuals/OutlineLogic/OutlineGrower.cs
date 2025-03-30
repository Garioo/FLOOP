using System.Collections;
using UnityEngine;

public class OutlineGrower : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public Vector3 thicknes;

    void Start()
    {
        transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        StartCoroutine(GrowOutline());
    }

    public IEnumerator GrowOutline()
    {
        float scale = 1f;
        while (scale < 1.1f)
        {
            scale += 0.02f;
            transform.localScale = new Vector3(scale, scale, scale);
            yield return new WaitForSeconds(0.1f);
        }
    }


}
