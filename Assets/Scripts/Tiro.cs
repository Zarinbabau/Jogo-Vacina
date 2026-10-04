using UnityEngine;

public class Tiro : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private Vector3 spawnPosition;

    public float speed = 25f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        spawnPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // Alterado para Vector3 para manter a estabilidade no mundo 3D
        Vector3 position = rigidbody.position;
        position += Vector3.up * speed * Time.fixedDeltaTime;

        rigidbody.MovePosition(position);
    }

    private void OnTriggerEnter(Collider other)
    {
        int layerBateu = other.gameObject.layer;

        // 1. Se bater no Player ou na barreira invisível, a bala ignora e continua
        if (layerBateu == LayerMask.NameToLayer("Player") ||
            layerBateu == LayerMask.NameToLayer("Ignore Raycast"))
        {
            return;
        }

        // 2. Se bater num cogumelo, destrói o cogumelo
        if (layerBateu == LayerMask.NameToLayer("Mushroom"))
        {
            Destroy(other.gameObject);
        }

        // NOVO: Se bater na Centopeia, interrompe o código aqui. 
        // O script CorpoCentopeia.cs será o responsável por destruir a bala.
        if (layerBateu == LayerMask.NameToLayer("Centopeia"))
        {
            return;
        }

        // 3. Independentemente de ser cogumelo ou parede, a bala destrói-se ao bater
        Destroy(gameObject);
    }

    public void Respawn()
    {
        transform.position = spawnPosition;

        // Correção de sintaxe para ativar o objeto corretamente
        gameObject.SetActive(true);
    }
}