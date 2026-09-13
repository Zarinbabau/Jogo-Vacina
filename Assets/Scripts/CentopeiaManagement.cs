using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CentopeiaManagement : MonoBehaviour
{
    private List<CorpoCentopeia> segments = new List<CorpoCentopeia>();
    
    public CorpoCentopeia segmentPrefab;
    
    public GameObject headSprite;
    public GameObject bodySprite;

    public int size = 12;

    private void Start()
    {
        Respawn();    
    }

    private void Respawn()
    {
        foreach (CorpoCentopeia segment in segments)
        {
            Destroy(segment.gameObject);
        }
        segments.Clear();

        for (int i = 0; i < size; i++)
        {
            Vector2 position = GridPosition(transform.position) + (Vector2.left * i);
            CorpoCentopeia segment = Instantiate(segmentPrefab, position, Quaternion.identity);
            segments.Add(segment);
        }
    }

    private Vector2 GridPosition(Vector2 position)
    {
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        return position;
    }
}
