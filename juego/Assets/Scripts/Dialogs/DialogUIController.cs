using System.Collections;
using Patterns.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUIController : MonoBehaviour
{
    public Image dialogBox; // Panel/Image que contiene el diálogo
    public TextMeshProUGUI dialogText; // Text component para mostrar el diálogo
    public Button nextButton; // Botón opcional para avanzar
    public Image characterImage;

    public float textSpeed = 0.05f; // Velocidad de aparición del texto (en segundos)
    public bool showInstantly = false; // Si false, anima el texto letra por letra

    [SerializeField]
    private char _redTextColorPrefix = '#'; // Carácter especial para cambiar el color del texto a rojo
    private Coroutine textCoroutine;
    private Canvas dialogCanvas;

    public void Initialize()
    {
        dialogCanvas = GetComponent<Canvas>();
        nextButton.onClick.AddListener(() =>
            ServiceLocator.Instance.GetService<DialogManager>().AdvanceDialog()
        );
        dialogCanvas.enabled = false;
    }

    public void ShowText(string text, Sprite characterSprite, int currentLine, int totalLines)
    {
        // Validar inicialización
        if (dialogCanvas == null)
        {
            Debug.LogError("DialogUIController: dialogCanvas no está asignado en el Inspector!");
            return;
        }
        dialogCanvas.enabled = true;

        // Asegurarse de que el DialogBox esté activo
        if (!dialogBox.gameObject.activeInHierarchy)
        {
            //Debug.Log("[DialogUIController] Activando DialogBox");
            dialogBox.gameObject.SetActive(true);
        }

        // FORZAR el color del texto a blanco visible
        if (dialogText != null)
        {
            dialogText.color = new Color(1f, 1f, 1f, 1f); // Blanco con alpha completo
            //Debug.Log("[DialogUIController] Color del texto establecido a: " + dialogText.color);
        }

        characterImage.enabled = true;
        characterImage.sprite = characterSprite;

        // Detener coroutine anterior si existe
        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }

        // Animar texto
        if (showInstantly)
        {
            dialogText.text = text;
            //Debug.Log("[DialogUIController] Texto asignado instantáneamente: " + text);
        }
        else
        {
            textCoroutine = StartCoroutine(AnimateText(text));
            //Debug.Log("[DialogUIController] Iniciando animación de texto");
        }
    }

    private IEnumerator AnimateText(string fullText)
    {
        dialogText.text = "";
        string textToAnimate = fullText;

        // Verificar si el texto comienza con el carácter especial para el color rojo
        if (
            !string.IsNullOrEmpty(fullText)
            && fullText.Length > 0
            && fullText[0] == _redTextColorPrefix
        )
        {
            dialogText.color = Color.darkRed; // Cambiar el color a rojo
            textToAnimate = fullText.Substring(1); // Omitir el primer carácter
        }
        else
        {
            dialogText.color = Color.white; // Cambiar el color a blanco
        }

        foreach (char letter in textToAnimate)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void HideUI()
    {
        if (dialogCanvas == null)
        {
            Debug.LogWarning("[DialogUIController] HideUI: canvasGroup es null");
            return;
        }

        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }

        characterImage.enabled = false;
        dialogCanvas.enabled = false;
    }

    /// <summary>
    /// Establece la velocidad de animación del texto
    /// </summary>
    public void SetTextSpeed(float speed)
    {
        textSpeed = speed;
    }
}
