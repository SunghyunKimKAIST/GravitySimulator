using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SandBoxManager : GameManager
{
    protected override void Start()
    {
        base.Start();

        particleUI[0].onValueChanged.AddListener(s =>
        {
            if (float.TryParse(s, out float f))
                transforms[focusing].position = new Vector2(f, transforms[focusing].position.y);
        });
        particleUI[1].onValueChanged.AddListener(s =>
        {
            if (float.TryParse(s, out float f))
                transforms[focusing].position = new Vector2(transforms[focusing].position.x, f);
        });
        particleUI[2].onValueChanged.AddListener(s =>
        {
            if (float.TryParse(s, out float f))
                lines[focusing].SetPosition(1, new Vector2(f, lines[focusing].GetPosition(1).y));
        });
        particleUI[3].onValueChanged.AddListener(s =>
        {
            if (float.TryParse(s, out float f))
                lines[focusing].SetPosition(1, new Vector2(lines[focusing].GetPosition(1).x, f));
        });
    }

    public override void SimulationStart()
    {
        foreach (InputField ui in particleUI)
            ui.readOnly = true;

        base.SimulationStart();
    }

    public override void Restart()
    {
        SceneManager.LoadScene(1);
    }
}