using System.Collections; // Necessário para usar Coroutines
using UnityEngine;

public class Player : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private Collider playerCollider;
    private Renderer playerRenderer; // Controla a visibilidade do modelo 3D
    private Vector2 direction;

    public float speed = 20f;

    // Tiro
    public GameObject tiroPrefab;
    public Transform pontoDeTiro;
    public float cooldown = 0.5f;
    private float tempoAtual;

    // Invulnerabilidade
    private bool isInvulneravel = false;
    public float tempoInvulnerabilidade = 0.5f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();

        // Pega o renderizador (se o modelo 3D for filho do objeto principal, use GetComponentInChildren<Renderer>())
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer == null)
        {
            playerRenderer = GetComponentInChildren<Renderer>();
        }
    }

    private void Update()
    {
        direction.x = Input.GetAxis("Horizontal");
        direction.y = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Fire1") && tempoAtual <= 0)
        {
            GameObject tiro = Instantiate(tiroPrefab, pontoDeTiro.position, pontoDeTiro.rotation);

            Collider tiroCollider = tiro.GetComponent<Collider>();
            if (playerCollider != null && tiroCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, tiroCollider);
            }

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

        position.x = Mathf.Clamp(position.x, -9f, 9f);
        position.y = Mathf.Clamp(position.y, -4f, -1f);

        rigidbody.MovePosition(position);
    }

    // Deteta o impacto com a centopeia
    private void OnCollisionEnter(Collision collision)
    {
        // Se bater na centopeia e NÃO estiver invulnerável
        if (!isInvulneravel && collision.gameObject.layer == LayerMask.NameToLayer("Centopeia"))
        {
            ReceberDano();
        }
    }

    // Caso a centopeia esteja configurada como Trigger, use este método também por segurança
    private void OnTriggerEnter(Collider other)
    {
        if (!isInvulneravel && other.gameObject.layer == LayerMask.NameToLayer("Centopeia"))
        {
            ReceberDano();
        }
    }

    private void ReceberDano()
    {
        GameManager.instance.PerderVida();
        StartCoroutine(RotinaInvulnerabilidade());
    }

    // Rotina que faz o jogador piscar e controla o tempo de invulnerabilidade
    private IEnumerator RotinaInvulnerabilidade()
    {
        isInvulneravel = true;
        float tempoPassado = 0f;

        // Enquanto não passar os 0.5 segundos...
        while (tempoPassado < tempoInvulnerabilidade)
        {
            // Inverte a visibilidade do modelo (se está visível, esconde; se está escondido, mostra)
            playerRenderer.enabled = !playerRenderer.enabled;

            // Espera 0.1 segundos antes de piscar de novo
            yield return new WaitForSeconds(0.1f);
            tempoPassado += 0.1f;
        }

        // Garante que no final ele fica visível e vulnerável novamente
        playerRenderer.enabled = true;
        isInvulneravel = false;
    }
}