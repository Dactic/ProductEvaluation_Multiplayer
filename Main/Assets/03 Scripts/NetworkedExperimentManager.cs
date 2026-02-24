using Fusion;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NetworkedExperimentManager : NetworkBehaviour
{
    [Header("Questions")]
    public GameObject[] questions;

    [Header("Sliders")]
    public Slider evaluationSlider;
    public Slider confidenceSliderP1;
    public Slider confidenceSliderP2;
    public Slider confidenceSliderP3;

    [Networked] public int CurrentQuestion { get; set; }

    [Networked] public float ValueEvaluation { get; set; }
    [Networked] public float ValueConfidenceP1 { get; set; }
    [Networked] public float ValueConfidenceP2 { get; set; }
    [Networked] public float ValueConfidenceP3 { get; set; }

    // Timer local por casco
    public float Timer = 0f;

    // Listas locales para guardar respuestas
    public List<float> evaluationResponses = new List<float>();
    public List<float> confidenceResponsesP1 = new List<float>();
    public List<float> confidenceResponsesP2 = new List<float>();
    public List<float> confidenceResponsesP3 = new List<float>();

    private string filename;

    [Header("Data exported")]
    public bool dataExported = false;

    // ----------------------------
    // SPAWNED
    // ----------------------------
    public override void Spawned()
    {
        filename = Application.persistentDataPath + "/Responses.csv";

        if (HasStateAuthority)
            CurrentQuestion = 0;

        // Conectar sliders a RPCs
        evaluationSlider.onValueChanged.AddListener((v) => RPC_SetSlider(1, v));
        confidenceSliderP1.onValueChanged.AddListener((v) => RPC_SetSlider(2, v));
        confidenceSliderP2.onValueChanged.AddListener((v) => RPC_SetSlider(3, v));
        confidenceSliderP3.onValueChanged.AddListener((v) => RPC_SetSlider(4, v));

        // Solo la primera pregunta visible
        for (int i = 0; i < questions.Length; i++)
            questions[i].SetActive(i == 0);
    }

    // ----------------------------
    // TIMER
    // ----------------------------
    public void FixedUpdate()
    {
        if (CurrentQuestion < questions.Length)
            Timer += Time.fixedDeltaTime;
    }

    // ----------------------------
    // BOTONES
    // ----------------------------
    public void NextQuestion()
    {
        // Guardar localmente antes de enviar RPC
        SaveLocalResponses();

        // Enviar RPC para avanzar pregunta y resetear sliders
        RPC_NextQuestion();
    }

    public void PreviousQuestion()
    {
        // Eliminar última respuesta local antes de enviar RPC
        RemoveLastResponses();

        // Enviar RPC para retroceder pregunta y resetear sliders
        RPC_PreviousQuestion();
    }

    // ----------------------------
    // RPC SLIDERS
    // ----------------------------
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_SetSlider(int id, float value)
    {
        switch (id)
        {
            case 1: ValueEvaluation = value; break;
            case 2: ValueConfidenceP1 = value; break;
            case 3: ValueConfidenceP2 = value; break;
            case 4: ValueConfidenceP3 = value; break;
        }
    }

    // ----------------------------
    // RPC BOTONES
    // ----------------------------
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_NextQuestion()
    {
        if (CurrentQuestion >= questions.Length) return;

        CurrentQuestion++;
        ResetSliders();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_PreviousQuestion()
    {
        if (CurrentQuestion <= 0) return;

        CurrentQuestion--;
        ResetSliders();
    }

    // ----------------------------
    // GUARDADO LOCAL
    // ----------------------------
    void SaveLocalResponses()
    {
        evaluationResponses.Add(ValueEvaluation);
        confidenceResponsesP1.Add(ValueConfidenceP1);
        confidenceResponsesP2.Add(ValueConfidenceP2);
        confidenceResponsesP3.Add(ValueConfidenceP3);

        // Exportar CSV si hemos terminado
        if (CurrentQuestion >= questions.Length - 1 && !dataExported)
        {
            ExportData();
            dataExported = true;
        }
    }

    void RemoveLastResponses()
    {
        if (evaluationResponses.Count > 0)
        {
            evaluationResponses.RemoveAt(evaluationResponses.Count - 1);
            confidenceResponsesP1.RemoveAt(confidenceResponsesP1.Count - 1);
            confidenceResponsesP2.RemoveAt(confidenceResponsesP2.Count - 1);
            confidenceResponsesP3.RemoveAt(confidenceResponsesP3.Count - 1);
        }
    }

    void ResetSliders()
    {
        ValueEvaluation = 0;
        ValueConfidenceP1 = 0;
        ValueConfidenceP2 = 0;
        ValueConfidenceP3 = 0;
    }

    // ----------------------------
    // RENDER
    // ----------------------------
    public override void Render()
    {
        // Actualizar sliders sin disparar eventos
        evaluationSlider.SetValueWithoutNotify(ValueEvaluation);
        confidenceSliderP1.SetValueWithoutNotify(ValueConfidenceP1);
        confidenceSliderP2.SetValueWithoutNotify(ValueConfidenceP2);
        confidenceSliderP3.SetValueWithoutNotify(ValueConfidenceP3);

        // Activar la pregunta correcta
        for (int i = 0; i < questions.Length; i++)
            questions[i].SetActive(i == CurrentQuestion);
    }

    // ----------------------------
    // EXPORT
    // ----------------------------
    void ExportData()
    {
        Debug.Log("DataExported");

        TextWriter tw = new StreamWriter(filename, false);

        tw.WriteLine("Scale" + ";" + "Evaluation" + ";" + "Confidence P1" + ";" + "Confidence P2" + ";" + "Confidence P3");

        for (int i = 0; i < evaluationResponses.Count; i++)
        {
            tw.WriteLine(
                questions[i].name + ";" +
                evaluationResponses[i] + ";" +
                confidenceResponsesP1[i] + ";" +
                confidenceResponsesP2[i] + ";" +
                confidenceResponsesP3[i]);
        }

        tw.WriteLine();
        tw.WriteLine("Timer");
        tw.WriteLine(Timer.ToString("F2"));

        tw.Close();
        Debug.Log("Local CSV exported on this headset: " + filename);
    }
}
