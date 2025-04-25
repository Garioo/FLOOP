using UnityEngine;

public class Sway : MonoBehaviour

{
    public float swayAmount = 5f; // degrees
    public float swaySpeed = 1f;

    void Update()
    {
        float angle = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

