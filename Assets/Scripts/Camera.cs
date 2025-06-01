using UnityEngine;

public class Camera : MonoBehaviour
{
    public Vector3 offset; // default is 0f, 0f, -10f
    public float smoothTime; // default is 0.25f
    private Vector3 velocity = Vector3.zero;
    public Transform target;
    public bool follow = false;

    private void Awake()
    {
        if (target)
            transform.position = target.position + offset;
    }

    private void Update()
    {
        if (target && follow)
            transform.position = Vector3.SmoothDamp(transform.position, target.position+offset, ref velocity, smoothTime);
    }
}

