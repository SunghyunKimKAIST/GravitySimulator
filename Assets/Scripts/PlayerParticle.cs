using UnityEngine;

public class PlayerParticle : Particle
{
    protected override void OnMouseDrag() { }

    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Finish":
                (gameManager as StageManager).Clear();
                break;

            case "Obstacle":
                (gameManager as StageManager).Gameover();
                break;
        }
    }
}