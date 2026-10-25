using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private Collider playerCollider;

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

    [Header("Efeitos Sonoros")]
    [Tooltip("Arraste o AudioSource 'hurt_player' aqui")]
    [SerializeField] private AudioSource hurtAudioSource;

    [Tooltip("Arraste o ARQUIVO de áudio da morte aqui")]
    [SerializeField] private AudioClip deathSoundClip;

    [Tooltip("Aumente este valor se o som da morte estiver muito baixo")]
    [Range(0f, 5f)]
    [SerializeField] private float deathVolume = 1f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    // Garante que, ao reiniciar o jogo, o player volte a ter colisão e fique visível
    private void OnEnable()
    {
        isInvulneravel = false;
        if (playerCollider != null) playerCollider.enabled = true;

        if (renderers != null)
        {
            foreach (Renderer r in renderers)
            {
                if (r != null) r.enabled = true;
            }
        }
    }

    private void Update()
    {
        // Se estiver "morto" tocando o som, impede de atirar
        if (playerCollider != null && !playerCollider.enabled) return;

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
        // Se estiver "morto" tocando o som, impede de se mover
        if (playerCollider != null && !playerCollider.enabled) return;

        Vector2 position = rigidbody.position;

        position += direction.normalized * speed * Time.fixedDeltaTime;

        position.x = Mathf.Clamp(position.x, -9f, 9f);
        position.y = Mathf.Clamp(position.y, -4f, -1f);

        rigidbody.MovePosition(position);
    }

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
        if (GameManager.instance.Lives <= 1)
        {
            StartCoroutine(TocarSomEMorrer());
        }
        else
        {
            if (hurtAudioSource != null)
            {
                hurtAudioSource.Play();
            }

            GameManager.instance.PerderVida();

            if (this.gameObject.activeInHierarchy)
            {
                StartCoroutine(RotinaInvulnerabilidade());
            }
        }
    }

    private IEnumerator TocarSomEMorrer()
    {
        isInvulneravel = true;
        if (playerCollider != null) playerCollider.enabled = false;

        foreach (Renderer r in renderers)
        {
            if (r != null) r.enabled = false;
        }

        float tempoDeEspera = 0.5f;
        if (deathSoundClip != null && hurtAudioSource != null)
        {
            hurtAudioSource.PlayOneShot(deathSoundClip, deathVolume);
            tempoDeEspera = deathSoundClip.length;
        }
        else if (deathSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(deathSoundClip, Camera.main.transform.position, deathVolume);
            tempoDeEspera = deathSoundClip.length;
        }

        yield return new WaitForSeconds(tempoDeEspera);

        GameManager.instance.PerderVida();
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