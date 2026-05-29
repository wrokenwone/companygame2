using UnityEngine;

public class MailClick : MonoBehaviour
{
    public GameObject applicationPanel;
    public DialogueManager dialogueManager;

    private string[] mailLines = {
        "En güçlü özelliğim? ",
        "Umutsuzluğun verdiği adaptasyon yeteneği.",
        "Çalışma şartlarını kabul etmiyorum",
        " ama reddedecek ekonomik özgürlüğüm de yok.",
        "...BAŞVURUYORUM!"
    };

    public void OpenApplication()
    {
        applicationPanel.SetActive(true);
        dialogueManager.StartDialogue(mailLines);
    }
}