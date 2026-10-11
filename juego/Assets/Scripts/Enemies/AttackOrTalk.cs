using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AttackOrTalk : MonoBehaviour, IInteractable
{
    [Header("Interfaz")]
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private Button attackButton;

    [SerializeField]
    private Button talkButton;

    [SerializeField]
    private Button cancelButton;

    [Header("Input del player")]
    [SerializeField]
    private InputReader input;

    private bool movementWasEnabled;

    private void OnEnable() { }

    private void OnDisable()
    {
        attackButton?.onClick.RemoveListener(Attack);
        talkButton?.onClick.RemoveListener(Talk);
        cancelButton?.onClick.RemoveListener(Cancel);
    }

    public void Interacted()
    {
        if (canvas == null)
        {
            Debug.LogError("AttackOrTalk: falta asignar el Canvas.", this);
            return;
        }

        canvas.enabled = true;
        input.DisableAllInput();
        attackButton?.onClick.AddListener(Attack);
        talkButton?.onClick.AddListener(Talk);
        cancelButton?.onClick.AddListener(Cancel);
    }

    public void Attack()
    {
        Debug.Log("Attacking enemy");
        SceneManager.LoadScene(10);
        CloseMenu();
        gameObject.tag = "Untagged";
    }

    public void Talk()
    {
        var talkComponent = GetComponent<TalkToEnemy>();

        if (talkComponent == null)
        {
            Debug.LogError("Talk Component no attachado");
            CloseMenu();
            return;
        }

        talkComponent.Talk();
        CloseMenu();
        gameObject.tag = "Untagged";
    }

    public void Cancel()
    {
        CloseMenu();
        input.EnableGameplayInput();
    }

    private void CloseMenu()
    {
        if (canvas != null)
            canvas.enabled = false;

        attackButton?.onClick.RemoveListener(Attack);
        talkButton?.onClick.RemoveListener(Talk);
        cancelButton?.onClick.RemoveListener(Cancel);
    }
}
