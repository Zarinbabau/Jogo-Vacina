using System.Collections.Generic;
using UnityEngine;

public class CentopeiaManagement : MonoBehaviour
{
    private List<GameObject> segments = new List<GameObject>();

    public GameObject headPrefab;
    public GameObject bodyPrefab;

    public float speed = 1f;
    public int size = 12;
    public LayerMask collisionMask;

    private void Start()
    {
        Respawn();
    }

    private void Respawn()
    {
        foreach (GameObject segment in segments)
        {
            Destroy(segment);
        }

        segments.Clear();

        for (int i = 0; i < size; i++)
        {
            Vector3 position = GridPosition(transform.position) + (Vector3.left * i);

            GameObject segment;

            if (i == 0)
            {
                segment = Instantiate(headPrefab, position, Quaternion.identity);
            }
            else
            {
                segment = Instantiate(bodyPrefab, position, Quaternion.identity);
            }

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

    private CorpoCentopeia GetSegmentAt(int index)
    {
        if (index >= 0 && index < segments.Count)
        {
            return segments[index].GetComponent<CorpoCentopeia>();
        }
        else
        {
            return null;
        }
    }

    private Vector3 GridPosition(Vector3 position)
    {
        position.x = Mathf.Round(position.x);
        position.z = Mathf.Round(position.z);

        return position;
    }
}