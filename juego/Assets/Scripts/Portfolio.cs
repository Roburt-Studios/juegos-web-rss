using UnityEngine;

public class Portfolio : MonoBehaviour
{
    [SerializeField] private string linkToPage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenLink()
    {
        Application.OpenURL(linkToPage); //Cuando se haga click en el botón, al jugador se le abrirá el portfolio del desarrollador corespondiente
    }
}
