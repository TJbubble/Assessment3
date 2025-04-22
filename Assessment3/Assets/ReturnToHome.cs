using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToHome : MonoBehaviour
{
    public void GoToHomePage()
    {
        SceneManager.LoadScene("HomePage"); 
    }
}