using System.Collections;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class GameManager : MonoBehaviour
{
    public string pipeName;
    public int maxBuff;

    public Transform[] transforms;
    public LineRenderer[] lines;

    public GameObject progressCircle;

    public InputField[] particleUI;
    protected int focusing;
    public int Focusing
    {
        set
        {
            focusing = value;
            SetParticleUI();
        }
    }

    public GameObject endButton;

    public float duration;
    protected virtual float _duration => duration;

    const string FLOAT_FORMAT = "F8";

    NamedPipeClientStream stream;
    StreamWriter writer;
    StreamReader reader;

    char[] buffer;
    bool simulating;
    Task<int> calcEnded;

    bool simulationOnce;

    public void SetParticleUI()
    {
        particleUI[0].text = transforms[focusing].position.x.ToString(FLOAT_FORMAT);
        particleUI[1].text = transforms[focusing].position.y.ToString(FLOAT_FORMAT);
        particleUI[2].text = lines[focusing].GetPosition(1).x.ToString(FLOAT_FORMAT);
        particleUI[3].text = lines[focusing].GetPosition(1).y.ToString(FLOAT_FORMAT);
    }

    protected virtual void Start()
    {
        Debug.Assert(transforms.Length == lines.Length);
        Debug.Assert(particleUI.Length == 4);

        buffer = new char[maxBuff];
        simulating = false;
        calcEnded = null;

        Focusing = 0;
        simulationOnce = false;
    }

    public virtual void SimulationStart()
    {
        if (simulationOnce)
            return;
        simulationOnce = true;

        stream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.None);
        try
        {
            stream.Connect();
        }
        catch (Win32Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.StackTrace);
            return;
        }
        Debug.Log("Connected to server");

        writer = new StreamWriter(stream);
        reader = new StreamReader(stream);

        writer.AutoFlush = true;

        StringBuilder writeString = new StringBuilder();

        writeString.AppendLine(_duration.ToString());
        for (int i = 0; i < transforms.Length; i++)
        {
            writeString.AppendLine(transforms[i].position.x.ToString(FLOAT_FORMAT));
            writeString.AppendLine(transforms[i].position.y.ToString(FLOAT_FORMAT));
            writeString.AppendLine(lines[i].GetPosition(1).x.ToString(FLOAT_FORMAT));
            writeString.AppendLine(lines[i].GetPosition(1).y.ToString(FLOAT_FORMAT));
        }
        writer.Write(writeString);
        Debug.Log("Writed initial values");

        calcEnded = reader.ReadAsync(buffer, 0, maxBuff);
        progressCircle.SetActive(true);
    }

    void Update()
    {
        if (!simulating && calcEnded != null && calcEnded.IsCompleted)
        {
            // "calc ended"
            Debug.Assert(calcEnded.Result == 10);
            simulating = true;
            calcEnded = null;
            progressCircle.SetActive(false);

            foreach(Transform tf in transforms)
                tf.GetComponent<TrailRenderer>().enabled = true;
        }
    }

    void FixedUpdate()
    {
        if (!simulating)
            return;

        int len;
        if ((len = reader.Read(buffer, 0, maxBuff)) == 0)
        {
            Debug.Log("End");
            simulating = false;
            StartCoroutine(AtEnd());
            return;
        }

        string[] resp = new string(buffer, 0, len).Split('\n');

        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i].position = new Vector2(float.Parse(resp[4 * i]), float.Parse(resp[4 * i + 1]));
            lines[i].SetPosition(1, new Vector2(float.Parse(resp[4 * i + 2]), float.Parse(resp[4 * i + 3])));
        }

        SetParticleUI();
    }

    public virtual IEnumerator AtEnd()
    {
        yield return new WaitForSeconds(1);
        endButton.SetActive(true);
    }

    public void Back()
    {
        SceneManager.LoadScene(0);
    }

    public abstract void Restart();
}