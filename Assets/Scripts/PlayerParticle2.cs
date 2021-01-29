using UnityEngine;

public class PlayerParticle2 : Particle
{
    public float velocity;

    protected override void OnMouseDrag() { }

    void Update()
    {
        rightDraging = rightDraging && Input.GetMouseButton(1);

        if (rightDraging && !started)
        {
            line.SetPosition(1, ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized * velocity);
            gameManager.SetParticleUI();
        }
    }

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