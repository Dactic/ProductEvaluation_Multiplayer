using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PositionRecorder : MonoBehaviour
{
    public GameObject player;
    private int frequence = 120;
    private float realFrequency;

    private bool dataExported = false;
    private string filename;

    private NetworkedExperimentManager manager;

    private List<float> posX = new();
    private List<float> posY = new();

    void Start()
    {
        filename = Application.persistentDataPath + $"/Position_{player.name}.csv";

        manager = FindFirstObjectByType<NetworkedExperimentManager>();

        realFrequency = 1f / frequence;
        InvokeRepeating(nameof(RecordPosition), 0, realFrequency);
    }

    void Update()
    {
        if (manager == null)
            return;

        // Exportar cuando el experimento ha terminado localmente
        if (!dataExported && manager.CurrentQuestion >= manager.questions.Length)
        {
            ExportData();
            dataExported = true;
            Debug.Log("Position data exported: " + filename);
        }
    }

    void RecordPosition()
    {
        if (manager == null || dataExported) return;

        posX.Add(player.transform.position.x);
        posY.Add(player.transform.position.z);
    }

    void ExportData()
    {
        using TextWriter tw = new StreamWriter(filename, false);

        tw.WriteLine("X;Y");

        for (int i = 0; i < posX.Count; i++)
            tw.WriteLine(posX[i] + ";" + posY[i]);
    }
}

