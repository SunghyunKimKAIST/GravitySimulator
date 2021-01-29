using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : GameManager
{
    public GameObject clearButton;

    Text endButtonText;

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
        if (!endButton.activeSelf)
        {
            endButtonText.text = "Time limit failure";
            endButton.SetActive(true);
        }
    }

    public void Clear()
    {
        if(!endButton.activeSelf)
            clearButton.SetActive(true);
    }

    public void Gameover()
    {
        endButton.SetActive(true);
    }

    public override void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}