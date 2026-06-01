using UnityEngine;
using UnityEngine.SceneManagement; // Sahneleri yönetmek için bu kütüphane şart

public class EndingManager : MonoBehaviour
{
    // Ana menü butonuna tıklanınca çalışacak fonksiyon
    public void GoToMainMenu()
    {
        // "MainMenu" yazan yere kendi ana menü sahnenin tam adını yazmalısın
        SceneManager.LoadScene("MainMenu"); 
    }

    // Credits butonuna tıklanınca çalışacak fonksiyon
    public void GoToCredits()
    {
        // "Credits" yazan yere kendi credits sahnenin tam adını yazmalısın
        SceneManager.LoadScene("Credits");
    }
}