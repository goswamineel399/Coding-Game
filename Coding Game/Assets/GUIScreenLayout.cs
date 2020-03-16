using System;
using UnityEngine;
using UnityEngine.UI;

public class GUIScreenLayout : MonoBehaviour
{
    public Rect model;
    public Rect consoleModel;

    [TextArea(0, 30)]
    public string textArea;
    [TextArea(0, 30)]
    public string consoleArea;

    public Button compileButton;

    public Executer executer;

    void OnGUI()
    {
        textArea = GUI.TextArea(new Rect(model.x, model.y, model.width, model.height), textArea);
        consoleArea = GUI.TextArea(new Rect(consoleModel.x, consoleModel.y, consoleModel.width, consoleModel.height), consoleArea);
    }

    void Start()
    {
        compileButton.onClick.AddListener( () => {
            try
            {
                consoleArea = executer.Execute(textArea);
            }
            catch (Exception e)
            {
                consoleArea = "Exception caught.\nExecution failed\n\n" + e.Message + "\n\n" + e.StackTrace;
            }
        });
    }
}
