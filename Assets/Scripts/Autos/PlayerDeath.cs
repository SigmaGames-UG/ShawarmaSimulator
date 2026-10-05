using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    public void Morir()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }
}