using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = transform.forward * 10f;
    }

    private void Update()
    {
        
    }
}
