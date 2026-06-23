using TourverseToolkit.Runtime;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public PlayerCamera playerCamera;
    void Start()
    {
        TourController.TourStart(playerCamera);
        TourController.CheckPoint(1);
    }
}
