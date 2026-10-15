using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CentopeiaManagement : MonoBehaviour
{
    private List<GameObject> segments = new List<GameObject>();

    [Header("Prefabs")]
    public GameObject headPrefab;
    public GameObject bodyPrefab;
    public GameObject mushroomPrefab;

    [Header("Configurações Iniciais")]
    public float speed = 1f;
    public int size = 12;

    public BoxCollider homeArea;
    public LayerMask collisionMask;

    [Header("Configurações de Dificuldade (Por Onda)")]
    [Tooltip("Valor somado à velocidade da centopeia a cada nova onda.")]
    public float aumentoDeVelocidade = 1.5f;

    [Tooltip("Quantidade de segmentos adicionados à centopeia a cada nova onda.")]
    public int aumentoDeTamanho = 2;

    [Tooltip("Multiplicador da vida da centopeia a cada nova onda (Ex: 1.05 = +5%).")]
    public float multiplicadorDeVida = 1.05f;

    // --- VARIÁVEIS DE CONTROLE INTERNO ---
    private float originalSpeed;
    private int originalSize;
    private float originalHealth = 1f; // Valor padrão inicial
    private float currentHealth;
    private bool initialized = false;

    // Salva os valores que você definiu no Unity Inspector para usar de base
    private void InitializeStats()
    {
        if (!initialized)
        {
            originalSpeed = speed;
            originalSize = size;

            // Pega a vida original diretamente do prefab do corpo
            if (bodyPrefab != null)
            {
                CorpoCentopeia corpo = bodyPrefab.GetComponent<CorpoCentopeia>();
                if (corpo != null)
                {
                    originalHealth = corpo.vidaSegmento;
                }
            }

            currentHealth = originalHealth;
            initialized = true;
        }
    }

    // Volta os valores ao normal se o tempo acabar ou o jogador perder todas as vidas
    public void ResetarDificuldade()
    {
        InitializeStats();
        speed = originalSpeed;
        size = originalSize;
        currentHealth = originalHealth;
    }

    // Aumenta a força da centopeia usando as variáveis públicas definidas no Inspector
    public void AumentarDificuldade()
    {
        speed += aumentoDeVelocidade;
        size += aumentoDeTamanho;
        currentHealth *= multiplicadorDeVida;
    }

    public void Respawn()
    {
        // Garante que os status iniciais foram capturados
        if (!initialized) InitializeStats();

        foreach (GameObject segment in segments)
        {
            Destroy(segment);
        }
        segments.Clear();

        Quaternion rotacaoInicial = Quaternion.Euler(-90f, 0f, 0f);

        for (int i = 0; i < size; i++)
        {
            Vector3 position = GridPosition(transform.position) + (Vector3.left * i);
            GameObject segment;

            if (i == 0)
                segment = Instantiate(headPrefab, position, rotacaoInicial);
            else
                segment = Instantiate(bodyPrefab, position, rotacaoInicial);

            CorpoCentopeia corpo = segment.GetComponent<CorpoCentopeia>();
            corpo.centipede = this;

            // Aplica a vida multiplicada (arredondando para int, garantindo no mínimo 1 de vida)
            corpo.vidaSegmento = Mathf.Max(1, Mathf.RoundToInt(currentHealth));

            segments.Add(segment);
        }

        for (int i = 0; i < segments.Count; i++)
        {
            CorpoCentopeia segment = segments[i].GetComponent<CorpoCentopeia>();
            segment.ahead = GetSegmentAt(i - 1);
            segment.behind = GetSegmentAt(i + 1);
        }
    }

    public void Remove(CorpoCentopeia segment)
    {
        // Calcula a posição do grid e trava o eixo Z em 0
        Vector3 position = GridPosition(segment.transform.position);
        position.z = 0f;

        // Spawna o cogumelo e aplica a escala exata
        GameObject novoCogumelo = Instantiate(mushroomPrefab, position, mushroomPrefab.transform.rotation);
        novoCogumelo.transform.localScale = new Vector3(0.8f, 0.25f, 0.6f);

        // Desconecta o segmento atingido da parte da frente
        if (segment.ahead != null)
        {
            segment.ahead.behind = null;
        }

        // O segmento de trás vira uma nova cabeça
        if (segment.behind != null)
        {
            CorpoCentopeia oldBody = segment.behind;
            oldBody.ahead = null;

            // Instancia a nova cabeça na posição exata do corpo antigo
            GameObject newHeadObj = Instantiate(headPrefab, oldBody.transform.position, oldBody.transform.rotation);
            CorpoCentopeia newHeadScript = newHeadObj.GetComponent<CorpoCentopeia>();

            // Transfere as lógicas e conexões
            newHeadScript.centipede = this;
            newHeadScript.behind = oldBody.behind;

            // HERANÇA VITAL: A nova cabeça mantém o fluxo da antiga
            newHeadScript.direction = oldBody.direction;
            newHeadScript.verticalDirection = oldBody.verticalDirection;

            // Herda a vida do segmento anterior para que não volte a ter a vida cheia caso já estivesse danificado
            newHeadScript.vidaSegmento = oldBody.vidaSegmento;

            if (newHeadScript.behind != null)
            {
                newHeadScript.behind.ahead = newHeadScript;
            }

            segments.Add(newHeadObj);
            segments.Remove(oldBody.gameObject);
            Destroy(oldBody.gameObject);
        }

        // 5. Remove a peça original atingida
        segments.Remove(segment.gameObject);
        Destroy(segment.gameObject);

        // LÓGICA DE PONTUAÇÃO (150 Cabeça / 100 Corpo)
        if (GameManager.instance != null)
        {
            int scorePoints = segment.isHead ? 150 : 100;
            GameManager.instance.AddScore(scorePoints);
        }

        // Verifica se já não sobra nenhum segmento na lista
        if (segments.Count == 0)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.CentopeiaDerrotada();
            }
        }
    }

    private CorpoCentopeia GetSegmentAt(int index)
    {
        if (index >= 0 && index < segments.Count)
        {
            return segments[index].GetComponent<CorpoCentopeia>();
        }
        return null;
    }

    private Vector3 GridPosition(Vector3 position)
    {
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        position.z = Mathf.Round(position.z);
        return position;
    }
}