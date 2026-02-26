using Fusion;
using UnityEngine;

public class Manager : NetworkBehaviour
{
    public NetworkObject product; // Debe tener NetworkObject + NetworkedTransform
    public NetworkObject canvas;  // Solo canvas se sincroniza networked

    [Header("Experiment has started")]
    [Networked] public NetworkBool EnableExperimentFlag { get; set; }

    // Vector3 networked desglosado para el canvas
    [Networked] public float CanvasPosX { get; set; }
    [Networked] public float CanvasPosY { get; set; }
    [Networked] public float CanvasPosZ { get; set; }

    // Posiciones visibles del experimento
    private Vector3 canvasPos = new Vector3(0, 1.75f, 1.5f);
    private Vector3 productPos = new Vector3(0, 1.065f, 1);

    // Posición inicial/oculta
    private Vector3 hiddenPos = new Vector3(0, -10f, 0);

    // ----------------------------
    // INICIALIZACIÓN
    // ----------------------------
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            // Inicializar canvas oculto
            CanvasPosX = hiddenPos.x;
            CanvasPosY = hiddenPos.y;
            CanvasPosZ = hiddenPos.z;

            EnableExperimentFlag = false;

            // Colocar producto en posición oculta físicamente
            if (product != null)
            {
                product.transform.position = hiddenPos;

                // Aseguramos que el producto pueda ser manipulado
                Rigidbody rb = product.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.isKinematic = true; // permite mover/rotar libremente
            }
        }
    }

    // ----------------------------
    // ENABLE / DISABLE EXPERIMENT
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
        // Mover canvas a posición visible
        CanvasPosX = canvasPos.x;
        CanvasPosY = canvasPos.y;
        CanvasPosZ = canvasPos.z;

        // Teleportar producto a posición visible
        if (product != null)
            product.transform.position = productPos;

        EnableExperimentFlag = true;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void DisableExperimentRpc()
    {
        // Ocultar canvas
        CanvasPosX = hiddenPos.x;
        CanvasPosY = hiddenPos.y;
        CanvasPosZ = hiddenPos.z;

        // Ocultar producto
        if (product != null)
            product.transform.position = hiddenPos;

        EnableExperimentFlag = false;
    }

    // ----------------------------
    // RENDER
    // ----------------------------
    public override void Render()
    {
        // Actualizar canvas networked
        if (canvas != null)
            canvas.transform.position = new Vector3(CanvasPosX, CanvasPosY, CanvasPosZ);

        // Producto no se actualiza aquí para permitir manipulación física libre
    }
}