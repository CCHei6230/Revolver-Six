using UnityEngine;
using UnityEngine.Serialization;

public class UIObjectBase : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;
    protected Vector3 posOnScreen;
    public bool fixPosition;
    [SerializeField] protected Camera cam;

    void Start()
    {
        cam = FindFirstObjectByType<Camera>();
    }

    void Update()
    {
        if (!Target) return;
        posOnScreen = cam.WorldToScreenPoint(Target.position + Offset);
        if (transform.position == posOnScreen) return;

        if (fixPosition)
        {
            transform.position = posOnScreen;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, posOnScreen, Time.deltaTime * 6f);
        }
    }
}
