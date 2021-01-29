using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public void SandBox()
    {
        SceneManager.LoadScene(1);
    }

    public void Stage1()
    {
        SceneManager.LoadScene(2);
    }

    public void Stage2()
    {
        SceneManager.LoadScene(3);
    }
}