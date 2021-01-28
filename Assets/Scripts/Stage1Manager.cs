using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage1Manager : GameManager
{
    Text endButtonText;

    public GameObject clearButton;

    protected override void Start()
    {
        base.Start();

        foreach (InputField ui in particleUI)
            ui.readOnly = true;

        endButtonText = endButton.GetComponentInChildren<Text>();
    }

    public override IEnumerator AtEnd()
    {
        yield return new WaitForSeconds(1);
        endButtonText.text = "Time limit failure";
        endButton.SetActive(true);
    }

    public IEnumerator Clear()
    {
        yield return new WaitForSeconds(1);
        clearButton.SetActive(true);
    }

    public void Gameover()
    {
        endButton.SetActive(true);
    }

    public override void Restart()
    {
        SceneManager.LoadScene(2);
    }
}