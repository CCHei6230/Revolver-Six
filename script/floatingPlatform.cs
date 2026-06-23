using UnityEngine;
using DG.Tweening;

public class floatingPlatform : MonoBehaviour
{
    public enum Pattern
    {
        Circle,
        AtoB,
        AtoBWithCircle
    }
    [SerializeField] Pattern pattern ;
    [SerializeField] float speed;
    [SerializeField] float radius;
    [SerializeField] Transform point0;
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] Vector2 point0Position;
    [SerializeField] Vector2 pointAPosition;
    [SerializeField] Vector2 pointBPosition;
    void Start()
    {
        if (point0)
        {
            point0Position = point0.position;
        }

        if (pointA)
        {
            pointAPosition = pointA.position;
        }

        if (pointB)
        {
            pointBPosition = pointB.position;
        }
    }

    void FixedUpdate()
    {
        switch (pattern)
        {
            case Pattern.Circle :
                Circle();
                break;
            case Pattern.AtoB :
                AtoB();
                break;
        }
    }

    void Circle()
    {
        transform.position = point0Position + new  Vector2( Mathf.Sin(Time.fixedTime * speed) * radius, Mathf.Cos(Time.fixedTime* speed) * radius);
    }
    void AtoB()
    {
        transform.position = point0Position + new  Vector2( Mathf.Sin(Time.fixedTime * speed) * radius, 0);
    }
}
