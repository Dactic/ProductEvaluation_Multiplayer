using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR.Input;

public class UserResponsesMultiplayer : MonoBehaviour
{
    [Header("Semantic scales")]
    public GameObject[] questions;

    public GameObject[] sliders;

    public Slider evaluationSlider; // Responses are taken from here

    public Slider confidenceSliderP1; // Responses are taken from here
    public Slider confidenceSliderP2;
    public Slider confidenceSliderP3;

    [Header("Responses")]
    public List<float> evaluationResponses = new List<float>();

    public List<float> confidenceResponsesP1 = new List<float>();
    public List<float> confidenceResponsesP2 = new List<float>();
    public List<float> confidenceResponsesP3 = new List<float>();

    private string filename;
    private int counter = 0;
    public bool dataExported = false;

    private void Start()
    {
        for (int i = 1; i < questions.Length; i++)
        {
            questions[i].SetActive(false);
        }
    }

    private void Update()
    {
        // To PC

        filename = Application.streamingAssetsPath + "/Responses.csv";

        // To headset
        //filename = Application.persistentDataPath + "/Responses.csv";
    }

    public void NextSemanticScale()
    {
        if (counter < questions.Length)
        {
            evaluationResponses.Add(evaluationSlider.value);

            confidenceResponsesP1.Add(confidenceSliderP1.value);
            confidenceResponsesP2.Add(confidenceSliderP2.value);
            confidenceResponsesP3.Add(confidenceSliderP3.value);

            // Reset values

            evaluationSlider.value = 0; // Neutral value

            confidenceSliderP1.value = 0; // Neutral value
            confidenceSliderP2.value = 0;
            confidenceSliderP3.value = 0;
                
            questions[counter].SetActive(false);
            counter++;

            if (counter < questions.Length)
            {
                questions[counter].SetActive(true);
            }

            else
            {
                //Change canvas position, as disabling it stops data exportation
                gameObject.transform.position = new Vector3(0, -2, 0);
            }

            if (counter == questions.Length)
            {
                // END HERE
                ExportData();
                dataExported = true;
            }
        }
    }

    public void PreviousSemanticScale()
    {
        if (counter >= 1)
        {
            evaluationResponses.RemoveAt(counter - 1);

            confidenceResponsesP1.RemoveAt(counter - 1);
            confidenceResponsesP2.RemoveAt(counter - 1);
            confidenceResponsesP3.RemoveAt(counter - 1);

            // Reset values

            evaluationSlider.value = 0; // Neutral value

            confidenceSliderP1.value = 0; // Neutral value
            confidenceSliderP2.value = 0;
            confidenceSliderP3.value = 0;

            questions[counter].SetActive(false);
            counter--;
            questions[counter].SetActive(true);
        }
    }

    public void ExportData()
    {
        TextWriter tw = new StreamWriter(filename, false);

        tw.WriteLine("Scale" + ";" + "Evaluation" + ";" + "Confidence P1" + ";" + "Confidence P2" + ";" + "Confidence P3");

        for (int i = 0; i < evaluationResponses.Count; i++)
        {
            tw.WriteLine(questions[i].name + ";" + evaluationResponses[i] + ";" + confidenceResponsesP1[i] + ";" + confidenceResponsesP2[i] + ";" + confidenceResponsesP3[i]);
        }

        tw.Close();

        Debug.Log("Evaluation data exported");
    }
}