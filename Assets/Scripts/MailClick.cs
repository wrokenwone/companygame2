using UnityEngine;

public class MailClick : MonoBehaviour
{
    public GameObject applicationPanel;

    public void OpenApplication()
    {
        applicationPanel.SetActive(true);
    }
}