using System.Collections.Generic;
using UnityEngine;

public class MushroomField : MonoBehaviour
{
    // A lista agora guarda GameObjects genéricos em vez de procurar por um script
    private List<GameObject> mushrooms;

    private BoxCollider area;
    public GameObject mushroomPrefab;
    public int mushroomCount = 10;

    private void Awake()
    {
        area = GetComponent<BoxCollider>();
        mushrooms = new List<GameObject>();
    }

    public void Generate()
    {
        Bounds bounds = area.bounds;

        for (int i = 0; i < mushroomCount; i++)
        {
            Vector3 position = Vector3.zero;

            position.x = Mathf.Round(Random.Range(bounds.min.x, bounds.max.x));
            position.y = Mathf.Round(Random.Range(bounds.min.y, bounds.max.y));
            position.z = 0f;

            // 1. Removemos o 'transform' do final do Instantiate. 
            // Assim, eles nascem soltos no mundo e ignoram a escala do Field.
            GameObject novoCogumelo = Instantiate(mushroomPrefab, position, mushroomPrefab.transform.rotation);

            // 2. Cravamos a mesma escala exata que o código da Centopeia usa!
            novoCogumelo.transform.localScale = new Vector3(0.8f, 0.25f, 0.6f);

            mushrooms.Add(novoCogumelo);
        }
    }

    public void Clear()
    {
        // Destrói todos os GameObjects da lista
        foreach (GameObject mushroom in mushrooms)
        {
            if (mushroom != null)
            {
                Destroy(mushroom);
            }
        }

        // Esvazia a lista para a próxima ronda
        mushrooms.Clear();
    }
}