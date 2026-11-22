using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CombatEncounter))]
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

    [SerializeField]
    private BattleManager battleManager;

    [SerializeField]
    private Transform playerTransform;

    void Update()
    {
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            720f * Time.deltaTime
        );
    }

    private void OnDisable()
    {
        CloseMenu();
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
        CombatEncounter encounter = GetComponent<CombatEncounter>();
        if (battleManager == null || encounter == null || !battleManager.BeginEncounter(encounter))
        {
            Debug.LogError("AttackOrTalk: encuentro o BattleManager sin configurar.", this);
            return;
        }
        CloseMenu();
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
