using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Exit"))
        {
            SceneManager.LoadScene("Victory");
        }else if (hit.gameObject.CompareTag("Die"))
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
