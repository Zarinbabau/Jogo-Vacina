using System.Collections.Generic;
using UnityEngine;

public class CentopeiaManagement : MonoBehaviour
{
    private List<CorpoCentopeia> segments = new List<CorpoCentopeia>();
    
    public CorpoCentopeia segmentPrefab;
    
    public GameObject headSprite;
    public GameObject bodySprite;

    private void Start()
    {
        Respawn();    
    }

    private void Respawn()
    { 

    }
}
