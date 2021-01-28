using UnityEngine;

public class Particle : MonoBehaviour
{
    LineRenderer line;

    bool rightDraging;

    void Start()
    {
        line = GetComponentInChildren<LineRenderer>();

        rightDraging = false;
    }

    void OnMouseDrag()
    {
        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseOver()
    {
        rightDraging = Input.GetMouseButton(1);
    }

    void Update()
    {
        rightDraging = rightDraging && Input.GetMouseButton(1);

        if (rightDraging)
            line.SetPosition(1, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position);
    }
}