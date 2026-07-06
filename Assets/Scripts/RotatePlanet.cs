using UnityEngine;

public class RotatePlanet : MonoBehaviour
{
    public float speed = 15f;

    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}