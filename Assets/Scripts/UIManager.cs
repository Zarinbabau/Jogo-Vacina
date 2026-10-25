using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoretext;
    [SerializeField] private TextMeshProUGUI timerText;

    // NOVO: Referências para as imagens de vida
    [Header("Imagens de Vida")]
    [Tooltip("Coloque a imagem 'coração' aqui")]
    [SerializeField] private GameObject vida1;

    [Tooltip("Coloque a imagem 'coração (1)' aqui")]
    [SerializeField] private GameObject vida2;

    [Tooltip("Coloque a imagem 'coração (2)' aqui")]
    [SerializeField] private GameObject vida3;

    void Start()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnScoreChanged -= UpdateScoreText;
            GameManager.instance.OnScoreChanged += UpdateScoreText;

            GameManager.instance.OnTimeChanged -= UpdateTimerText;
            GameManager.instance.OnTimeChanged += UpdateTimerText;

            // NOVO: Eventos de Vida
            GameManager.instance.OnLivesChanged -= UpdateLivesUI;
            GameManager.instance.OnLivesChanged += UpdateLivesUI;
        }
    }

    public void RefreashAllUI()
    {
        if (GameManager.instance != null)
        {
            UpdateScoreText(GameManager.instance.score);
            UpdateTimerText(GameManager.instance.TempoRestante);

            // NOVO: Força atualizar as vidas caso seja chamado
            UpdateLivesUI(GameManager.instance.Lives);
        }
    }

    private void UpdateScoreText(int newScore)
    {
        if (scoretext != null)
        {
            scoretext.text = $"Score: {newScore}";
        }
    }

    private void UpdateTimerText(float tempoEmSegundos)
    {
        if (timerText != null)
        {
            int minutos = Mathf.FloorToInt(tempoEmSegundos / 60);
            int segundos = Mathf.FloorToInt(tempoEmSegundos % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    // NOVO: Método para ligar/desligar os corações de acordo com a vida atual
    private void UpdateLivesUI(int currentLives)
    {
        // Se a vida for maior ou igual a 1, o coração 1 fica ativo, senão desativa.
        if (vida1 != null) vida1.SetActive(currentLives >= 1);

        // Se a vida for maior ou igual a 2, o coração 2 fica ativo, senão desativa.
        if (vida2 != null) vida2.SetActive(currentLives >= 2);

        // Se a vida for 3, o coração 3 fica ativo, senão desativa.
        if (vida3 != null) vida3.SetActive(currentLives >= 3);
    }
}