using UnityEngine;
using TMPro;
using System.Collections;

public class CompassDevice : MonoBehaviour
{
    public TextMeshPro coordinatesText;

    public void ActivateCompass()
    {
        StartCoroutine(GenerateCoordinates());
    }

    IEnumerator GenerateCoordinates()
    {
        coordinatesText.text = "Сканирование...";
        yield return new WaitForSeconds(2f);
        
        float x1 = Random.Range(10f, 99f);
        float x2 = Random.Range(100f, 999f);
        float x3 = Random.Range(100f, 999f);
        float x4 = Random.Range(1f, 9f);

        coordinatesText.text = $"{x1:F0}.{x2:F0}.{x3:F0}.{x4:F0}";
    }
}