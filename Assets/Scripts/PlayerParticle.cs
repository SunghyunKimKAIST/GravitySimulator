using UnityEngine;

public class PlayerParticle : Particle
{
    protected override void OnMouseDrag() { }

    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Finish":
                StartCoroutine((gameManager as Stage1Manager).Clear());
                break;

            case "Obstacle":
                (gameManager as Stage1Manager).Gameover();
                break;
        }
    }
}