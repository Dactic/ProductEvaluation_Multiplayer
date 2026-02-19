using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class NetworkedSliders : NetworkBehaviour
{
    public Slider evaluationSlider;
    public Slider confidenceSliderP1;
    public Slider confidenceSliderP2;
    public Slider confidenceSliderP3;

    [Networked] public float Value1 { get; set; }
    [Networked] public float Value2 { get; set; }
    [Networked] public float Value3 { get; set; }
    [Networked] public float Value4 { get; set; }

    public override void Spawned()
    {
        evaluationSlider.onValueChanged.AddListener((v) => RPC_SetSlider(1, v));
        confidenceSliderP1.onValueChanged.AddListener((v) => RPC_SetSlider(2, v));
        confidenceSliderP2.onValueChanged.AddListener((v) => RPC_SetSlider(3, v));
        confidenceSliderP3.onValueChanged.AddListener((v) => RPC_SetSlider(4, v));
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_SetSlider(int id, float value)
    {
        switch (id)
        {
            case 1: Value1 = value; break;
            case 2: Value2 = value; break;
            case 3: Value3 = value; break;
            case 4: Value4 = value; break;
        }
    }

    public override void Render()
    {
        evaluationSlider.SetValueWithoutNotify(Value1);
        confidenceSliderP1.SetValueWithoutNotify(Value2);
        confidenceSliderP2.SetValueWithoutNotify(Value3);
        confidenceSliderP3.SetValueWithoutNotify(Value4);
    }
}

