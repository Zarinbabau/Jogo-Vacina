using UnityEngine;

public class CameraSway : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;

    [Header("Configurações de Rotação")]
    public float anguloMaximo = 15f;
    public float velocidadeSuavidade = 5f; // Quão rápido a câmera acompanha a rotação

    // Limites baseados no Mathf.Clamp do script Player.cs
    private float limiteEsquerdo = -9f;
    private float limiteDireito = 9f;

    // Divisões dos terços
    private float tercoEsquerdo = -3f;
    private float tercoDireito = 3f;

    private float anguloAlvoY = 0f;
    private float anguloAtualY = 0f;
    private Quaternion rotacaoInicial;

    private void Start()
    {
        // Salva a rotação inicial (0,0,0) para garantir que os eixos X e Z não sejam afetados
        rotacaoInicial = transform.rotation;
    }

    private void Update()
    {
        if (player == null) return;

        float playerX = player.position.x;

        // Verifica em qual terço o player está e calcula a porcentagem de inclinação
        if (playerX > tercoDireito) // Está no terço direito (3 a 9)
        {
            // InverseLerp retorna um valor de 0 a 1 indicando o quão fundo ele está no terço
            float porcentagem = Mathf.InverseLerp(tercoDireito, limiteDireito, playerX);
            anguloAlvoY = porcentagem * anguloMaximo;
        }
        else if (playerX < tercoEsquerdo) // Está no terço esquerdo (-9 a -3)
        {
            float porcentagem = Mathf.InverseLerp(tercoEsquerdo, limiteEsquerdo, playerX);
            anguloAlvoY = porcentagem * -anguloMaximo;
        }
        else // Está no terço central (-3 a 3)
        {
            anguloAlvoY = 0f;
        }

        // Suaviza a transição do ângulo atual para o ângulo alvo (Lerp)
        anguloAtualY = Mathf.Lerp(anguloAtualY, anguloAlvoY, Time.deltaTime * velocidadeSuavidade);

        // Aplica a rotação apenas no eixo Y, somada à rotação que a câmera já possuía
        transform.rotation = Quaternion.Euler(
            rotacaoInicial.eulerAngles.x,
            rotacaoInicial.eulerAngles.y + anguloAtualY,
            rotacaoInicial.eulerAngles.z
        );
    }
}