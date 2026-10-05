using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI scoretext;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SubscribeToEvents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SubscribeToEvents()
    {
        if (GameManager.instance != null)
        {
           GameManager.instance.OnScoreChanged -= UpdateScoreText;

           GameManager.instance.OnScoreChanged += UpdateScoreText;
        }
    }

    public void RefreashAllUI()
    {
        if (GameManager.instance !=null)
        {
            UpdateScoreText (GameManager.instance.score);
        }
    }

    private void UpdateScoreText(int newScore)
    {
        if (scoretext != null)
        {
            scoretext.text = $"Score: {newScore}";
        }
    }
}
