using UnityEngine;
using System.Collections;

public class PressButtonAnimation : MonoBehaviour
{
    [SerializeField] private Transform buttonVisual;
    [SerializeField] private Vector3 pressOffset = new Vector3(0f, -0.02f, -0.03f);
    [SerializeField] private float pressTime = 0.08f;
    [SerializeField] private float returnTime = 0.12f;

    private Vector3 startLocalPosition;
    private Coroutine pressRoutine;

    private void Awake()
    {
        startLocalPosition = buttonVisual.localPosition;
    }

    public void PlayPressAnimation()
    {
        if (pressRoutine != null)
            StopCoroutine(pressRoutine);

        pressRoutine = StartCoroutine(PressRoutine());
    }

    private IEnumerator PressRoutine()
    {
        Vector3 pressedPosition = startLocalPosition + pressOffset;

        float timer = 0f;

        while (timer < pressTime)
        {
            timer += Time.deltaTime;
            buttonVisual.localPosition = Vector3.Lerp(
                startLocalPosition,
                pressedPosition,
                timer / pressTime
            );

            yield return null;
        }

        buttonVisual.localPosition = pressedPosition;

        timer = 0f;

        while (timer < returnTime)
        {
            timer += Time.deltaTime;
            buttonVisual.localPosition = Vector3.Lerp(
                pressedPosition,
                startLocalPosition,
                timer / returnTime
            );

            yield return null;
        }

        buttonVisual.localPosition = startLocalPosition;
    }
}
