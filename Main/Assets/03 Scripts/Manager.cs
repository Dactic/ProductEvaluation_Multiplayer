using Fusion;
using UnityEngine;

public class Manager : NetworkBehaviour
{
    public NetworkObject product; // Debe tener NetworkObject + NetworkedTransform
    public NetworkObject canvas;  // Igual

    [Header("Experiment has started")]
    [Networked] public NetworkBool EnableExperimentFlag { get; set; }

    // Vector3 networked desglosado
    [Networked] public float CanvasPosX { get; set; }
    [Networked] public float CanvasPosY { get; set; }
    [Networked] public float CanvasPosZ { get; set; }

    [Networked] public float ProductPosX { get; set; }
    [Networked] public float ProductPosY { get; set; }
    [Networked] public float ProductPosZ { get; set; }

    // Posiciones iniciales
    private Vector3 canvasPos = new Vector3(0, 1.75f, 1.5f);
    private Vector3 productPos = new Vector3(0, 1.065f, 1);

    // ----------------------------
    // Enable/Disable Experiment
    // ----------------------------
    public void EnableExperiment()
    {
        if (HasStateAuthority)
            EnableExperimentRpc();
    }

    public void DisableExperiment()
    {
        if (HasStateAuthority)
            DisableExperimentRpc();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void EnableExperimentRpc()
    {
        CanvasPosX = canvasPos.x;
        CanvasPosY = canvasPos.y;
        CanvasPosZ = canvasPos.z;

        ProductPosX = productPos.x;
        ProductPosY = productPos.y;
        ProductPosZ = productPos.z;

        EnableExperimentFlag = true;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void DisableExperimentRpc()
    {
        CanvasPosX = 0; CanvasPosY = -10; CanvasPosZ = 0;
        ProductPosX = 0; ProductPosY = -10; ProductPosZ = 0;
        EnableExperimentFlag = false;
    }

    public override void Render()
    {
        if (canvas != null)
            canvas.transform.position = new Vector3(CanvasPosX, CanvasPosY, CanvasPosZ);
        if (product != null)
            product.transform.position = new Vector3(ProductPosX, ProductPosY, ProductPosZ);
    }

    public void ExperimentStart()
    {
        if (HasStateAuthority)
            EnableExperimentFlag = true;
    }
}