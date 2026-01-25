using UnityEngine;

public class Chest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Open()
    {
        GetComponent<Animator>().SetTrigger("Open");
    }
}
