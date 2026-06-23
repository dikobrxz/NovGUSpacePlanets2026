using TourverseToolkit.Runtime;
using UnityEngine;

internal class MM : MonoBehaviour
{
    SS s;

    [Inject]
    private void Construct(SS ss)
    {
        s = ss;
    }

    [ContextMenu("ASD")]
    public void SSSS()
    {
        s.DE();
    }
}
