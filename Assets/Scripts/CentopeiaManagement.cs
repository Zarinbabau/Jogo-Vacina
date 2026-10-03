using System.Collections.Generic;
using UnityEngine;

public class CentopeiaManagement : MonoBehaviour
{
    private List<GameObject> segments = new List<GameObject>();

    public GameObject headPrefab;
    public GameObject bodyPrefab;
    public GameObject mushroomPrefab;

    public float speed = 1f;
    public int size = 12;

    public BoxCollider homeArea;
    public LayerMask collisionMask;

    public void Respawn()
    {
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
        // Calcula a posi��o do grid e trava o eixo Z em 0
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

        // O segmento de tr�s vira uma nova cabe�a
        if (segment.behind != null)
        {
            CorpoCentopeia oldBody = segment.behind;
            oldBody.ahead = null;

            // Instancia a nova cabe�a na posi��o exata do corpo antigo
            GameObject newHeadObj = Instantiate(headPrefab, oldBody.transform.position, oldBody.transform.rotation);
            CorpoCentopeia newHeadScript = newHeadObj.GetComponent<CorpoCentopeia>();

            // Transfere as l�gicas e conex�es
            newHeadScript.centipede = this;
            newHeadScript.behind = oldBody.behind;

            // HERAN�A VITAL: A nova cabe�a mant�m o fluxo da antiga
            newHeadScript.direction = oldBody.direction;
            newHeadScript.verticalDirection = oldBody.verticalDirection;

            if (newHeadScript.behind != null)
            {
                newHeadScript.behind.ahead = newHeadScript;
            }

            segments.Add(newHeadObj);
            segments.Remove(oldBody.gameObject);
            Destroy(oldBody.gameObject);
        }

        // 5. Remove a pe�a original atingida
        segments.Remove(segment.gameObject);
        Destroy(segment.gameObject);

        // --- NOVO: Verifica se j� n�o sobra nenhum segmento na lista ---
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