using TourverseToolkit.Runtime;
using UnityEngine;

internal class MMM : MonoBehaviour
{
    MM s;

    [Inject]
    private void Construct(MM ss)
    {
        s = ss;
    }

    [ContextMenu("ASD")]
    public void SSSS()
    {
        s.SSSS();
    }
}