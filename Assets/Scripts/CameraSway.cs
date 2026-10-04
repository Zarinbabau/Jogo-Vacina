using UnityEngine;

public class CameraSway : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;

    [Header("Configurações de Rotação")]
    public float anguloMaximo = 15f;
    public float velocidadeSuavidade = 5f;

    [Header("Configurações de Posição")]
    public Vector3 posicaoCentro = new Vector3(0f, 0f, -20f);
    public Vector3 posicaoEsquerda = new Vector3(-5f, 0f, -15f);
    public Vector3 posicaoDireita = new Vector3(5f, 0f, -15f);

    private float limiteEsquerdo = -9f;
    private float limiteDireito = 9f;

    private float tercoEsquerdo = -3f;
    private float tercoDireito = 3f;

    private float anguloAlvoY = 0f;
    private Vector3 posicaoAlvo;

    private float anguloAtualY = 0f;
    private Vector3 posicaoAtual;
    private Quaternion rotacaoInicial;

    private void Start()
    {
        rotacaoInicial = transform.rotation;
        posicaoAtual = transform.position;
        posicaoAlvo = posicaoCentro;
    }

    private void Update()
    {
        if (player == null) return;

        float playerX = player.position.x;

        if (playerX > tercoDireito) // Terço direito (3 a 9)
        {
            float porcentagem = Mathf.InverseLerp(tercoDireito, limiteDireito, playerX);

            // INVERTIDO: Adicionado o sinal de menos (-) para olhar para o centro
            anguloAlvoY = porcentagem * -anguloMaximo;
            posicaoAlvo = Vector3.Lerp(posicaoCentro, posicaoDireita, porcentagem);
        }
        else if (playerX < tercoEsquerdo) // Terço esquerdo (-9 a -3)
        {
            float porcentagem = Mathf.InverseLerp(tercoEsquerdo, limiteEsquerdo, playerX);

            // INVERTIDO: Removido o sinal de menos para olhar para o centro
            anguloAlvoY = porcentagem * anguloMaximo;
            posicaoAlvo = Vector3.Lerp(posicaoCentro, posicaoEsquerda, porcentagem);
        }
        else // Terço central (-3 a 3)
        {
            anguloAlvoY = 0f;
            posicaoAlvo = posicaoCentro;
        }

        anguloAtualY = Mathf.Lerp(anguloAtualY, anguloAlvoY, Time.deltaTime * velocidadeSuavidade);
        transform.rotation = Quaternion.Euler(
            rotacaoInicial.eulerAngles.x,
            rotacaoInicial.eulerAngles.y + anguloAtualY,
            rotacaoInicial.eulerAngles.z
        );

        posicaoAtual = Vector3.Lerp(posicaoAtual, posicaoAlvo, Time.deltaTime * velocidadeSuavidade);
        transform.position = posicaoAtual;
    }
}