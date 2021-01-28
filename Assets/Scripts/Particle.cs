using UnityEngine;

public class Particle : MonoBehaviour
{
    public int index;
    public GameManager gameManager;

    bool started;

    LineRenderer line;

    bool rightDraging;

    void Start()
    {
        line = GetComponentInChildren<LineRenderer>();

        started = false;
        rightDraging = false;
    }

    public void SimulationStart()
    {
        started = true;
    }

    protected virtual void OnMouseDrag()
    {
        if (!started)
        {
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameManager.SetParticleUI();
        }
    }

    private void OnMouseOver()
    {
        gameManager.Focusing = index;

        rightDraging = Input.GetMouseButton(1);
    }

    void Update()
    {
        rightDraging = rightDraging && Input.GetMouseButton(1);

        if (rightDraging && !started)
        {
            line.SetPosition(1, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position);
            gameManager.SetParticleUI();
        }
    }
}