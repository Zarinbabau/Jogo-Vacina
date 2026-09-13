using UnityEngine;

public class Player : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private Vector2 direction;

    public float speed = 20f;

    // Tiro
    public GameObject tiroPrefab;
    public Transform pontoDeTiro;
    public float cooldown = 0.5f;

    private float tempoAtual;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        direction.x = Input.GetAxis("Horizontal");
        direction.y = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Fire1") && tempoAtual <= 0)
        {
            Instantiate(tiroPrefab, pontoDeTiro.position, pontoDeTiro.rotation);

            tempoAtual = cooldown;
        }

        if (tempoAtual > 0)
        {
            tempoAtual -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidbody.position;

        position += direction.normalized * speed * Time.fixedDeltaTime;

        // Limites da área
        position.x = Mathf.Clamp(position.x, -8.9f, 8.9f);
        position.y = Mathf.Clamp(position.y, -3.7f, -1f);

        rigidbody.MovePosition(position);
    }
}