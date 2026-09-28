using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    private Player player;
    private CentopeiaManagement centipedeManager;
    private MushroomField mushroomField;

    private int score;
    private int lives;

    public float tempoMaximo = 300f; // 5 minutos em segundos
    private float tempoRestante;

    private Vector3 playerOriginalPosition;

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

            if (tempoRestante <= 0)
            {
                tempoRestante = 0;
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

        score = 0;
        lives = 3;
        tempoRestante = tempoMaximo;

        centipedeManager.Respawn();

        if (player != null)
        {
            player.transform.position = playerOriginalPosition;
            player.gameObject.SetActive(true);
        }

        mushroomField.Clear();
        mushroomField.Generate();
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

    // NOVO: Chamado quando a centopeia é totalmente destruída
    public void CentopeiaDerrotada()
    {
        if (player != null)
        {
            player.gameObject.SetActive(false); // Opcional: trava o player ao limpar o nível
        }

        Debug.Log("Centopeia eliminada! Irá reiniciar o nível em 3 segundos.");

        // Reutiliza exatamente a mesma lógica de espera de 3 segundos
        Invoke(nameof(NewGame), 3f);
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
}