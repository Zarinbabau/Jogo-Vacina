using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoretext;

    // NOVO: Variável para receber o seu objeto TextMeshPro "Timer"
    [SerializeField] private TextMeshProUGUI timerText;

    void Start()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (GameManager.instance != null)
        {
            // Eventos de Score
            GameManager.instance.OnScoreChanged -= UpdateScoreText;
            GameManager.instance.OnScoreChanged += UpdateScoreText;

            // NOVO: Eventos de Tempo
            GameManager.instance.OnTimeChanged -= UpdateTimerText;
            GameManager.instance.OnTimeChanged += UpdateTimerText;
        }
    }

    public void RefreashAllUI()
    {
        if (GameManager.instance != null)
        {
            UpdateScoreText(GameManager.instance.score);

            // NOVO: Força atualizar o tempo caso seja chamado
            UpdateTimerText(GameManager.instance.TempoRestante);
        }
    }

    private void UpdateScoreText(int newScore)
    {
        if (scoretext != null)
        {
            scoretext.text = $"Score: {newScore}";
        }
    }

    // NOVO: Método que recebe os segundos e formata para a interface
    private void UpdateTimerText(float tempoEmSegundos)
    {
        if (timerText != null)
        {
            // Calcula os minutos e segundos arredondando para baixo
            int minutos = Mathf.FloorToInt(tempoEmSegundos / 60);
            int segundos = Mathf.FloorToInt(tempoEmSegundos % 60);

            // Formata o texto para sempre exibir 2 dígitos (ex: 05:09)
            timerText.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }
}