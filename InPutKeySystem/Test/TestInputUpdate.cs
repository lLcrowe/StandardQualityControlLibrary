using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class TestInputUpdate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var value1 = Input.GetKeyDown(KeyCode.None);
        

        var keyCode = UnityEngine.InputSystem.Key.None;
        var key = UnityEngine.InputSystem.Keyboard.current[keyCode];
        var value2 = key.wasPressedThisFrame;
        Debug.Log($"{value1}_{value2}");
    }
}
