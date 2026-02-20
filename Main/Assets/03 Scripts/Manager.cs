using Fusion;
using UnityEngine;

public class Manager : NetworkBehaviour
{
    public NetworkObject product; // debe tener NetworkObject y NetworkedTransform
    public NetworkObject canvas;  // igual
    public GameObject button;

    [Header("Experiment has started")]
    public bool enableExperiment = false;

    // Positions

    private Vector3 canvasPos = new Vector3(0, 1.75f, 1.5f);
    private Vector3 productPos = new Vector3(0, 1.065f, 1);

    public void EnableExperiment()
    {
        // Llama al RPC para todos
        EnableExperimentRpc();
        enableExperiment = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void EnableExperimentRpc()
    {
        canvas.transform.position = canvasPos;
        product.transform.position = productPos;
    }
}
