using System.Collections; // Necessário para usar Coroutines
using UnityEngine;

public class Player : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private Collider playerCollider;

    // Substituímos o Renderer único por um array de Renderers
    private Renderer[] renderers;

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

        // Pega todos os renderizadores no Player E nos objetos filhos (como o Quad)
        renderers = GetComponentsInChildren<Renderer>();
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
        if (!isInvulneravel && collision.gameObject.layer == LayerMask.NameToLayer("Centopeia"))
        {
            ReceberDano();
        }
    }

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

        // Só inicia a corotina de piscar se o GameManager não tiver desativado o Player
        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(RotinaInvulnerabilidade());
        }
    }

    // Rotina que faz o jogador piscar e controla o tempo de invulnerabilidade
    private IEnumerator RotinaInvulnerabilidade()
    {
        isInvulneravel = true;
        float tempoPassado = 0f;

        // Enquanto nao passar os 0.5 segundos...
        while (tempoPassado < tempoInvulnerabilidade)
        {
            // Inverte a visibilidade de todos os renderizadores encontrados (Player e Quad)
            foreach (Renderer r in renderers)
            {
                if (r != null)
                {
                    r.enabled = !r.enabled;
                }
            }

            // Espera 0.1 segundos antes de piscar de novo
            yield return new WaitForSeconds(0.1f);
            tempoPassado += 0.1f;
        }

        // Garante que no final todos fiquem visiveis e vulneraveis novamente
        foreach (Renderer r in renderers)
        {
            if (r != null)
            {
                r.enabled = true;
            }
        }
        isInvulneravel = false;
    }
}