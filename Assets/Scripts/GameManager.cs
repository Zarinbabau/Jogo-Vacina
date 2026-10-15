using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    private Player player;
    private CentopeiaManagement centipedeManager;
    private MushroomField mushroomField;
    private int lives;

    public float tempoMaximo = 300f; // 5 minutos em segundos
    private float tempoRestante;

    // NOVO: Propriedade pública para ler o tempo se necessário
    public float TempoRestante => tempoRestante;

    private Vector3 playerOriginalPosition;

    [SerializeField] public int score;

    public event Action<int> OnScoreChanged;
    public event Action<float> OnTimeChanged; // NOVO: Evento para avisar o UIManager

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        centipedeManager = FindAnyObjectByType<CentopeiaManagement>();
        mushroomField = FindAnyObjectByType<MushroomField>();

        if (player != null)
        {
            playerOriginalPosition = player.transform.position;
        }

        NewGame();
    }

    private void Update()
    {
        if (lives > 0 && tempoRestante > 0)
        {
            tempoRestante -= Time.deltaTime;

            // NOVO: Atualiza o tempo na UI a cada frame
            OnTimeChanged?.Invoke(tempoRestante);

            if (tempoRestante <= 0)
            {
                tempoRestante = 0;
                OnTimeChanged?.Invoke(tempoRestante); // Garante que o texto mostre 00:00
                lives = 0;
                GameOver();
            }
        }

        if (lives <= 0 && Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        CancelInvoke(nameof(NewGame));
        CancelInvoke(nameof(ProximaOnda));

        lives = 3;
        tempoRestante = tempoMaximo;

        score = 0;
        OnScoreChanged?.Invoke(score);

        // NOVO: Atualiza a UI do tempo assim que o jogo começar
        OnTimeChanged?.Invoke(tempoRestante);

        if (centipedeManager != null)
        {
            centipedeManager.ResetarDificuldade();
            centipedeManager.Respawn();
        }

        if (player != null)
        {
            player.transform.position = playerOriginalPosition;
            player.gameObject.SetActive(true);
        }

        if (mushroomField != null)
        {
            mushroomField.Clear();
            mushroomField.Generate();
        }
    }

    public void PerderVida()
    {
        lives--;

        if (lives <= 0)
        {
            lives = 0;
            GameOver();
        }
    }

    public void CentopeiaDerrotada()
    {
        if (player != null)
        {
            player.gameObject.SetActive(false);
        }

        Debug.Log("Centopeia eliminada! Próxima onda em 3 segundos.");
        Invoke(nameof(ProximaOnda), 3f);
    }

    private void ProximaOnda()
    {
        if (player != null)
        {
            player.transform.position = playerOriginalPosition;
            player.gameObject.SetActive(true);
        }

        if (centipedeManager != null)
        {
            centipedeManager.AumentarDificuldade();
            centipedeManager.Respawn();
        }
    }

    private void GameOver()
    {
        if (player != null)
        {
            player.gameObject.SetActive(false);
        }

        Debug.Log("Game Over! O jogo vai reiniciar em 3 segundos.");
        Invoke(nameof(NewGame), 3f);
    }

    public void AddScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);
    }
}