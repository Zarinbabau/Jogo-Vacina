using UnityEngine;

public class CorpoCentopeia : MonoBehaviour
{
    public CentopeiaManagement centipede { get; set; }
    public CorpoCentopeia ahead { get; set; }
    public CorpoCentopeia behind { get; set; }

    public bool isHead => ahead == null;

    public Vector3 direction = Vector3.right;
    public float verticalDirection = -1f;

    private Vector3 targetPosition;
    public int vidaSegmento;

    [Header("Configurações de Áudio")]
    public AudioSource AudioSource;       
    public AudioClip batidaClip;        
    public AudioClip virusMorteClip;    

    private void Awake()
    {
        targetPosition = transform.position;
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
    }

    private void Update()
    {
        if (isHead && Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            UpdateHeadSegment();
        }

        float speed = centipede.speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed
        );

        Vector3 movementDirection = targetPosition - transform.position;

        if (movementDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }
        else if (movementDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(-90f, 180f, 0f);
        }
    }

    public void UpdateHeadSegment()
    {
        Vector3 gridPosition = GridPosition(transform.position);

        targetPosition = gridPosition + direction;

        Collider[] collisions = Physics.OverlapBox(
            targetPosition,
            new Vector3(0.4f, 0.4f, 0.4f),
            Quaternion.identity,
            centipede.collisionMask
        );

        if (collisions.Length > 0)
        {
            // Toca o som de batida quando colide com a collision mask
            if (AudioSource != null && batidaClip != null)
            {
                AudioSource.PlayOneShot(batidaClip);
            }

            targetPosition = gridPosition + new Vector3(0f, verticalDirection, 0f);
            direction.x = -direction.x;

            Bounds homeBounds = centipede.homeArea.bounds;

            if ((verticalDirection == 1f && targetPosition.y > homeBounds.max.y) ||
                (verticalDirection == -1f && targetPosition.y < homeBounds.min.y))
            {
                verticalDirection = -verticalDirection;
                targetPosition.y = gridPosition.y + verticalDirection;
            }
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = GridPosition(transform.position) + direction;
        }

        if (behind != null)
        {
            behind.UpdateBodySegment();
        }
    }

    private void UpdateBodySegment()
    {
        targetPosition = GridPosition(ahead.transform.position);
        direction = ahead.direction;

        if (behind != null)
        {
            behind.UpdateBodySegment();
        }
    }

    private Vector3 GridPosition(Vector3 position)
    {
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        position.z = Mathf.Round(position.z);

        return position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Tiro"))
        {
            // Busca o script ShellExplosion no filho da bala ANTES de qualquer coisa
            ShellExplosion explosion = other.GetComponentInChildren<ShellExplosion>();

            // Reduz 1 de vida do segmento atingido
            vidaSegmento--;

            // Se a vida esgotar, detona e morre
            if (vidaSegmento <= 0)
            {
                // Detona a explosão ANTES do tiro ser destruído
                if (explosion != null)
                {
                    explosion.Detonate();
                }

                // Toca o som de morte do vírus
                if (virusMorteClip != null)
                {
                    AudioSource.PlayClipAtPoint(virusMorteClip, transform.position);
                }

                centipede.Remove(this);
            }

            // AGORA SIM, depois de ter detonado, desativamos e destruímos o tiro
            other.enabled = false;
            Destroy(other.gameObject);
        }
    }
}