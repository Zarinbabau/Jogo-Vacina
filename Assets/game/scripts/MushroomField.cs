using UnityEngine;

public class MushroomField : MonoBehaviour
{
    private BoxCollider area;
    public GameObject mushroomPrefab;
    public int mushroomCount = 10;


    private void Awake()
    {
        area = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        Generate();
    }

    private void Generate()
    {
    Bounds bounds = area.bounds;

        for (int i = 0; i < mushroomCount; i++)
        {
            Vector3 position = Vector3.zero;

            position.x = Mathf.Round(Random.Range(bounds.min.x, bounds.max.x));
            position.y = Mathf.Round(Random.Range(bounds.min.y, bounds.max.y));
            position.z = Mathf.Round(Random.Range(bounds.min.z, bounds.max.z));

            Instantiate(mushroomPrefab, position, Quaternion.identity, transform);
        }
    }
}