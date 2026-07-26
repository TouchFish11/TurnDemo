using UnityEngine;

public class CubeTest : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("CubeTest.Awake");
        Debug.Log(Time.frameCount);
    }

    private void OnEnable()
    {
        Debug.Log("CubeTest.OnEnable");
        Debug.Log(Time.frameCount);
    }
    
    void Start()
    {
        Debug.Log("CubeTest.Start");
        Debug.Log(Time.frameCount);
    }
}
