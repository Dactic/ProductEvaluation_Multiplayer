using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PositionRecorder : MonoBehaviour
{
    public GameObject player;
    private int frequence = 120;
    private float realFrequency;

    // Bools controlling data exportation

    public bool evaluationDataExported;
    private bool dataExported = false;

    private string filename;

    // Lists

    private List<float> posX = new();
    private List<float> posY = new();

    void Start()
    {
        // To PC

        filename = Application.streamingAssetsPath + "/Position.csv";

        // To headset
        //filename = Application.persistentDataPath + "/Position.csv";

        UpdateInvoke();
    }

    void UpdateInvoke()
    {
        if (frequence <= 0)
        {
            return;
        }

        realFrequency = 1f / frequence;
        CancelInvoke("RecordPosition");
        InvokeRepeating("RecordPosition", 0, realFrequency);
    }

    public void Update()
    {
        evaluationDataExported = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UserResponsesMultiplayer>().dataExported;

        if (evaluationDataExported && !dataExported)
        {
            ExportData();
            dataExported = true;

            Debug.Log("Position data exported.");
        }
    }

    void RecordPosition()
    {
        // Records data while evaluating product

        if (!evaluationDataExported)
        {
            posX.Add(player.transform.position.x);
            posY.Add(player.transform.position.z);
        }
    }

    void ExportData()
    {
        TextWriter tw = new StreamWriter(filename, false);

        tw.WriteLine("X" + ";" + "Y");

        for (int i = 0; i < posX.Count; i++)
        {
            tw.WriteLine(posX[i] + ";" + posY[i]);
        }

        tw.Close();
    }
}

