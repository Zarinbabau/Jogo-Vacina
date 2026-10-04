using UnityEngine;

public class LuzParallax : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        // Encontra a câmera principal do jogo automaticamente
        cam = Camera.main;
    }

    private void Update()
    {
        if (cam != null)
        {
            // Calcula a linha de visão exata da câmera até a posição atual do tiro
            Vector3 direcaoVisao = transform.position - cam.transform.position;

            // Faz o cone da Spot Light apontar exatamente para onde essa linha continua no fundo
            transform.forward = direcaoVisao;
        }
    }
}