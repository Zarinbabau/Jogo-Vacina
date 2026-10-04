using UnityEngine;

public class CentipedeExplosion : MonoBehaviour
{
    private ParticleSystem m_ExplosionParticles;
    private AudioSource m_ExplosionAudio;

    private void Awake()
    {
        // Busca os componentes automaticamente no próprio objeto em que o script está anexado
        m_ExplosionParticles = GetComponent<ParticleSystem>();
        m_ExplosionAudio = GetComponent<AudioSource>();
    }

    public void Detonate()
    {
        // 1. Desvincula o objeto "Explosion" do "Tiro", para que ele não suma quando a bala for destruída
        transform.parent = null;

        // 2. Toca as partículas e o som, caso existam
        if (m_ExplosionParticles != null)
        {
            m_ExplosionParticles.Play();
        }

        if (m_ExplosionAudio != null)
        {
            m_ExplosionAudio.Play();
        }

        // 3. Destrói este objeto de explosão após o tempo de duração das partículas terminar
        if (m_ExplosionParticles != null)
        {
            ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
            // Adiciona uma pequena margem de segurança ao tempo de destruição, garantindo que o som também termine
            Destroy(gameObject, mainModule.duration + 0.5f);
        }
        else
        {
            Destroy(gameObject, 2f); // Fallback de segurança se não houver partícula
        }
    }
}