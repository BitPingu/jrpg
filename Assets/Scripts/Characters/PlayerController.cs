using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float HorizontalInput { get; set; }
    public float VerticalInput { get; set; }

    public bool E { get; set; }
    public bool Q { get; set; }

    private void Update()
    {
        // horizontal and vertical input
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");

        // key inputs
        E = Input.GetKeyDown(KeyCode.E);
        Q = Input.GetKeyDown(KeyCode.Q);
    }
}

