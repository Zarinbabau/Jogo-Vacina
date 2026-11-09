using System.Collections; // Necessário para usar Coroutines
using UnityEngine;

public class Player : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private Collider playerCollider;

    // Referência para o Animator (agora busca também nos filhos)
    private Animator animator;

    // Array de Renderers nos filhos
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
        
        // CORREÇÃO: Usa GetComponentInChildren para encontrar o Animator no objeto filho (modelo 3D)
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator não encontrado nos filhos do Player!");
        }

        // Pega todos os renderizadores no objeto e nos filhos
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        direction.x = Input.GetAxis("Horizontal");
        direction.y = Input.GetAxis("Vertical");

        // Atualiza o parâmetro booleano de movimento no Animator
        bool moving = direction.sqrMagnitude > 0.01f;
        if (animator != null)
        {
            animator.SetBool("isMoving", moving);
        }

        if (Input.GetButtonDown("Fire1") && tempoAtual <= 0)
        {
            // Dispara a animação de tiro
            if (animator != null)
            {
                animator.SetTrigger("isShooting");
            }

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

        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(RotinaInvulnerabilidade());
        }
    }

    private IEnumerator RotinaInvulnerabilidade()
    {
        isInvulneravel = true;
        float tempoPassado = 0f;

        while (tempoPassado < tempoInvulnerabilidade)
        {
            foreach (Renderer r in renderers)
            {
                if (r != null)
                {
                    r.enabled = !r.enabled;
                }
            }

            yield return new WaitForSeconds(0.1f);
            tempoPassado += 0.1f;
        }

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