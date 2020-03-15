using UnityEngine;

public class Executer : MonoBehaviour
{
    [TextArea(0, 30)]
    public string code;
    // Start is called before the first frame update
    void Start()
    {
        var interpreter = new ESOLang(code.Split('\n'));
        Debug.Log(interpreter.execute());
    }
}
