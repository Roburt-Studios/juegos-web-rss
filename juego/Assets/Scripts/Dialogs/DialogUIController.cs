using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUIController : MonoBehaviour
{
    public Image dialogBox;
    public TextMeshProUGUI dialogText;
    public Button nextButton;
    public Image characterImage;
    public float textSpeed = 0.05f;
    public bool showInstantly = false;

    [SerializeField]
    private char _redTextColorPrefix = '#';
    private readonly List<GameObject> optionButtons = new();
    private Coroutine textCoroutine;
    private string fullText = "";
    private Canvas dialogCanvas;
    private DialogManager manager;

    public void Initialize(DialogManager dialogManager)
    {
        manager = dialogManager;
        dialogCanvas = GetComponent<Canvas>();
        nextButton.onClick.RemoveListener(manager.AdvanceDialog);
        nextButton.onClick.AddListener(manager.AdvanceDialog);
        dialogCanvas.enabled = false;
    }

    public void ShowNode(Dialog.Node node, Sprite characterSprite)
    {
        if (dialogCanvas == null)
        {
            Debug.LogError("DialogUIController: falt canvas o Initialize.");
            return;
        }

        ClearOptions();
        dialogCanvas.enabled = true;
        dialogBox.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(!node.isChoice);

        if (textCoroutine != null)
            StopCoroutine(textCoroutine);
        textCoroutine = null;

        if (node.isChoice)
        {
            fullText = "";
            dialogText.text = "";
            dialogText.enabled = false;
            characterImage.enabled = false;

            for (int i = 0; i < node.options.Count; i++)
            {
                int selectedIndex = i;
                Dialog.Option option = node.options[i];
                string label =
                    option.difficulty > 0
                        ? $"[2d6 ≥ {option.difficulty}] {option.text}"
                        : option.text;
                CreateActionButton(
                    label,
                    () => manager.SelectChoice(selectedIndex),
                    i,
                    node.options.Count,
                    true
                );
            }
            return;
        }

        dialogText.enabled = true;
        characterImage.enabled = characterSprite != null;
        characterImage.sprite = characterSprite;

        string text = node.text;
        bool red = !string.IsNullOrEmpty(text) && text[0] == _redTextColorPrefix;
        dialogText.color = red ? Color.red : Color.white;
        if (red)
            text = text.Substring(1);
        fullText = text;

        if (showInstantly || text.Length <= 1 || textSpeed <= 0f)
            dialogText.text = text;
        else
            textCoroutine = StartCoroutine(AnimateText(text));
    }

    public void ShowRollPrompt(int difficulty)
    {
        ClearOptions();
        if (textCoroutine != null)
            StopCoroutine(textCoroutine);
        textCoroutine = null;
        fullText = $"Tira 2d6. Necesitas sacar {difficulty} o más.";
        dialogText.enabled = true;
        dialogText.color = Color.white;
        dialogText.text = fullText;
        characterImage.enabled = false;
        nextButton.gameObject.SetActive(false);
        CreateActionButton("Tirar 2d6", manager.RollForChoice, 0, 1);
    }

    public void ShowRollResult(DiceRoll roll, int difficulty, bool success)
    {
        ClearOptions();
        fullText =
            $"Tirada: {roll.FirstDie} + {roll.SecondDie} = {roll.Total} / {difficulty}. "
            + (success ? "Éxito." : "Fallo.");
        dialogText.color = Color.white;
        dialogText.text = fullText;
        nextButton.gameObject.SetActive(false);
        CreateActionButton("Siguiente", manager.AdvanceDialog, 0, 1, true);
        RectTransform buttonRect = (RectTransform)optionButtons[0].transform;
        buttonRect.sizeDelta = new Vector2(720f, 32f);
        buttonRect.anchoredPosition = new Vector2(0f, 6f);
    }

    private void CreateActionButton(
        string label,
        Action action,
        int index,
        int count,
        bool insideDialogBox = false
    )
    {
        //TODO: Cambiar esto a un prefab
        var buttonObject = new GameObject(
            $"Respuesta {index + 1}",
            typeof(RectTransform),
            typeof(Image),
            typeof(Button)
        );
        buttonObject.transform.SetParent(transform, false);
        buttonObject.layer = gameObject.layer;

        RectTransform rect = (RectTransform)buttonObject.transform;
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        if (insideDialogBox)
        {
            float buttonHeight = Mathf.Min(46f, (112f - (count - 1) * 6f) / count);
            rect.sizeDelta = new Vector2(720f, buttonHeight);
            rect.anchoredPosition = new Vector2(
                0f,
                14f + (count - index - 1) * (buttonHeight + 6f)
            );
        }
        else
        {
            rect.sizeDelta = new Vector2(720f, 46f);
            rect.anchoredPosition = new Vector2(0f, 150f + (count - index - 1) * 54f);
        }

        Image background = buttonObject.GetComponent<Image>();
        background.color = new Color(0.08f, 0.08f, 0.08f, 0.95f);
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = background;
        button.onClick.AddListener(() => action());

        var labelObject = new GameObject("Texto", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);
        labelObject.layer = gameObject.layer;
        RectTransform labelRect = (RectTransform)labelObject.transform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(12f, 2f);
        labelRect.offsetMax = new Vector2(-12f, -2f);

        TextMeshProUGUI optionText = labelObject.GetComponent<TextMeshProUGUI>();
        optionText.font = dialogText.font;
        optionText.fontSize = dialogText.fontSize;
        optionText.color = Color.white;
        optionText.alignment = TextAlignmentOptions.Center;
        optionText.enableWordWrapping = true;
        optionText.raycastTarget = false;
        optionText.text = label;
        optionButtons.Add(buttonObject);
    }

    private IEnumerator AnimateText(string fullText)
    {
        dialogText.text = "";
        for (int i = 0; i < fullText.Length; i++)
        {
            dialogText.text += fullText[i];
            if (i < fullText.Length - 1)
                yield return new WaitForSeconds(textSpeed);
        }
        textCoroutine = null;
    }

    public bool CompleteCurrentText()
    {
        if (textCoroutine == null)
            return false;

        StopCoroutine(textCoroutine);
        textCoroutine = null;
        dialogText.text = fullText;
        return true;
    }

    private void ClearOptions()
    {
        foreach (GameObject button in optionButtons)
        {
            button.SetActive(false);
            Destroy(button);
        }
        optionButtons.Clear();
    }

    public void HideUI()
    {
        if (textCoroutine != null)
            StopCoroutine(textCoroutine);
        textCoroutine = null;
        fullText = "";
        ClearOptions();
        characterImage.enabled = false;
        if (dialogCanvas != null)
            dialogCanvas.enabled = false;
    }

    public void SetTextSpeed(float speed) => textSpeed = speed;
}
