using UnityEngine;

public class Executer : MonoBehaviour
{
    // Start is called before the first frame update
    public string Execute(string code)
    {
        var interpreter = new ESOLang(code.Split('\n'));
        return interpreter.execute();
    }
}
