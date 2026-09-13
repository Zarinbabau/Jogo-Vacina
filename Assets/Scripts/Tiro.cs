using UnityEngine;

public class Tiro : MonoBehaviour
{
    private new Rigidbody rigidbody;

    public float speed = 25f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidbody.position;
        position += Vector2.up * speed * Time.fixedDeltaTime;

        rigidbody.MovePosition(position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}