using System.Text;
using System.IO;
using System.IO.Pipes;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SandBoxManager : MonoBehaviour
{
    public string pipeName;
    public int maxBuff;

    public Transform[] transforms;
    public LineRenderer[] lines;

    public GameObject progressCircle;

    NamedPipeClientStream stream;
    StreamWriter writer;
    StreamReader reader;

    char[] buffer;
    bool simulating;
    Task<int> calcEnded;

    void Start()
    {
        Debug.Assert(transforms.Length == lines.Length);

        buffer = new char[maxBuff];
        simulating = false;
        calcEnded = null;
    }

    public void SimulationStart()
    {
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

        // writeString.AppendLine(particles.Length.ToString());
        for (int i = 0; i < transforms.Length; i++)
        {
            writeString.AppendLine(transforms[i].position.x.ToString());
            writeString.AppendLine(transforms[i].position.y.ToString());
            writeString.AppendLine(lines[i].GetPosition(1).x.ToString());
            writeString.AppendLine(lines[i].GetPosition(1).y.ToString());
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
            return;
        }

        string[] resp = new string(buffer, 0, len).Split('\n');

        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i].position = new Vector2(float.Parse(resp[4 * i]), float.Parse(resp[4 * i + 1]));
            lines[i].SetPosition(1, new Vector2(float.Parse(resp[4 * i + 2]), float.Parse(resp[4 * i + 3])));
        }
    }

    public void Back()
    {
        SceneManager.LoadScene(0);
    }
}