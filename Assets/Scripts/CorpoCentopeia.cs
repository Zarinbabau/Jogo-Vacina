using UnityEngine;

public class CorpoCentopeia : MonoBehaviour
{
    public CentopeiaManagement centipede { get; set; }
    public CorpoCentopeia ahead { get; set; }
    public CorpoCentopeia behind { get; set; }

    public bool isHead => ahead == null;

    private Vector3 direction = Vector3.right;
    private Vector3 targetPosition;

    private void Awake()
    {
        targetPosition = transform.position;
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
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (movementDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    private void UpdateHeadSegment()
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
            // Primeiro desce 1 unidade no Y
            targetPosition = gridPosition + Vector3.down;

            // Inverte o sentido horizontal
            direction.x = -direction.x;
        }

        // Depois que terminou de descer,
        // começa a andar horizontalmente no sentido oposto
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
}