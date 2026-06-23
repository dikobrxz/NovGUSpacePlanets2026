using UnityEngine;
using TourverseToolkit.Runtime;

public class NewMonoBehaviourScript : EntryPoint
{
    private SS _ss;
    [SerializeField] private MM mm;
    [SerializeField] private MMM mmm;

    protected override void InstallDependencies()
    {
        Container.Register<SS>();
        Container.Register(mm);
        Container.Register(mmm);
    }
}
