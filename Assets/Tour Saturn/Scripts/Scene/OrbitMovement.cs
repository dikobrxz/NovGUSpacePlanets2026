using UnityEngine;

public class OrbitMovement : MonoBehaviour
{
    [Header("Path Points")]
    [SerializeField] private Transform[] points;

    [Header("Movement")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool lookForward = true;

    private int currentPointIndex;
    private bool isMoving;

    public void StartMovement()
    {
        if (points == null || points.Length == 0)
        {
            return;
        }

        transform.position = points[0].position;
        currentPointIndex = 1;
        isMoving = true;
    }

    public void StopMovement()
    {
        isMoving = false;
    }

    private void Update()
    {
        if (!isMoving)
            return;

        Transform target = points[currentPointIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (lookForward)
        {
            Vector3 direction = target.position - transform.position;

            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);
        }

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            currentPointIndex++;

            if (currentPointIndex >= points.Length)
            {
                if (loop)
                    currentPointIndex = 0;
                else
                    isMoving = false;
            }
        }
    }
}
