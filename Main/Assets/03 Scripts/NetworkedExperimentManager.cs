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

    [Networked] public NetworkBool DataExported { get; set; }

    // Timer
    public float Timer = 0f;

    // Local response storage
    private List<float> evaluationResponses = new();
    private List<float> confidenceResponsesP1 = new();
    private List<float> confidenceResponsesP2 = new();
    private List<float> confidenceResponsesP3 = new();

    private string filename;
    private Manager manager;

    private bool lastExported = false;

    // ----------------------------
    // SPAWNED
    // ----------------------------
    public override void Spawned()
    {
        manager = FindFirstObjectByType<Manager>();

        filename = Application.persistentDataPath + $"/Responses_{Runner.LocalPlayer.PlayerId}.csv";

        if (HasStateAuthority)
            CurrentQuestion = 0;

        evaluationSlider.onValueChanged.AddListener((v) => RPC_SetSlider(1, v));
        confidenceSliderP1.onValueChanged.AddListener((v) => RPC_SetSlider(2, v));
        confidenceSliderP2.onValueChanged.AddListener((v) => RPC_SetSlider(3, v));
        confidenceSliderP3.onValueChanged.AddListener((v) => RPC_SetSlider(4, v));

        for (int i = 0; i < questions.Length; i++)
            questions[i].SetActive(i == 0);
    }

    // ----------------------------
    // TIMER (Fusion tick-based)
    // ----------------------------
    public override void FixedUpdateNetwork()
    {
        if (manager == null) return;

        if (manager.EnableExperimentFlag && !DataExported && CurrentQuestion < questions.Length)
            Timer += Runner.DeltaTime;
    }

    // ----------------------------
    // BUTTONS
    // ----------------------------
    public void NextQuestion()
    {
        SaveLocalResponses();
        RPC_NextQuestion();
    }

    public void PreviousQuestion()
    {
        RemoveLastResponses();
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
    // RPC QUESTIONS
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
    // SAVE LOCAL DATA
    // ----------------------------
    void SaveLocalResponses()
    {
        evaluationResponses.Add(ValueEvaluation);
        confidenceResponsesP1.Add(ValueConfidenceP1);
        confidenceResponsesP2.Add(ValueConfidenceP2);
        confidenceResponsesP3.Add(ValueConfidenceP3);

        if (CurrentQuestion >= questions.Length - 1 && !lastExported)
        {
            ExportData();
            lastExported = true;

            if (HasStateAuthority && manager != null)
                manager.DisableExperiment();
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
        // Detecta cambio de DataExported
        evaluationSlider.SetValueWithoutNotify(ValueEvaluation);
        confidenceSliderP1.SetValueWithoutNotify(ValueConfidenceP1);
        confidenceSliderP2.SetValueWithoutNotify(ValueConfidenceP2);
        confidenceSliderP3.SetValueWithoutNotify(ValueConfidenceP3);

        for (int i = 0; i < questions.Length; i++)
            questions[i].SetActive(i == CurrentQuestion);
    }

    // ----------------------------
    // EXPORT
    // ----------------------------
    void ExportData()
    {
        using TextWriter tw = new StreamWriter(filename, false);

        tw.WriteLine("Scale;Evaluation;Confidence P1;Confidence P2;Confidence P3");

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

        Debug.Log("CSV exportado en: " + filename);
    }
}
