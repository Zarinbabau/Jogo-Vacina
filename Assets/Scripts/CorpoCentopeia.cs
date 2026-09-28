using UnityEngine;

public class CorpoCentopeia : MonoBehaviour
{
    public CentopeiaManagement centipede { get; set; }
    public CorpoCentopeia ahead { get; set; }
    public CorpoCentopeia behind { get; set; }

    public bool isHead => ahead == null;

    private Vector3 direction = Vector3.right;
    private float verticalDirection = -1f;
    private Vector3 targetPosition;

    private void Awake()
    {
        targetPosition = transform.position;

        // Garante que todo segmento (cabeça ou corpo) já nasça deitado
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
            // Indo para a direita: mantém X em -90 e Y em 0
            transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }
        else if (movementDirection.x < 0)
        {
            // Indo para a esquerda: mantém X em -90 e Y em 180
            transform.rotation = Quaternion.Euler(-90f, 180f, 0f);
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
            // Move 1 unidade no Y dependendo se está subindo ou descendo
            targetPosition = gridPosition + new Vector3(0f, verticalDirection, 0f);

            // Inverte o sentido horizontal
            direction.x = -direction.x;

            // Inverter o eixo Y quando bater nos limites do home
            Bounds homeBounds = centipede.homeArea.bounds;

            if ((verticalDirection == 1f && targetPosition.y > homeBounds.max.y) ||
                (verticalDirection == -1f && targetPosition.y < homeBounds.min.y))
            {
                verticalDirection = -verticalDirection;
                targetPosition.y = gridPosition.y + verticalDirection;
            }
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