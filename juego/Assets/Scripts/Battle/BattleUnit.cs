
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [Header("Datos del personaje")]
    public string unitName = "Combatiente";
    public int maxHP = 100;
    public int currentHP = 100;
    public int attackPower = 15;
    public int skillPower = 25;

    public bool IsAlive => currentHP > 0;

    private Image image;
    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private Coroutine damageCoroutine;

    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        originalPosition = rectTransform.localPosition;

        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }


    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;

        currentHP = Mathf.Max(0, currentHP - damage);
        UpdateAppearance();
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;

        currentHP = Mathf.Min(maxHP, currentHP + amount);
        UpdateAppearance();
    }

    public IEnumerator Attack()
    {
        float duration = 0.15f;
        float elapsed = 0f;

        Vector2 startPosition = originalPosition;
        Vector2 attackPosition = originalPosition + new Vector2(0f, -40f);

        // Movimiento hacia abajo
        while (elapsed < duration)
        {
            float t = elapsed / duration;

            rectTransform.localPosition = Vector2.Lerp(
                startPosition,
                attackPosition,
                t
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = attackPosition;

        elapsed = 0f;

        // Regreso
        while (elapsed < duration)
        {
            float t = elapsed / duration;

            rectTransform.localPosition = Vector2.Lerp(
                attackPosition,
                startPosition,
                t
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = originalPosition;
    }

    public void SetSelected(bool selected)
    {
        if (image == null) return;

        if (!IsAlive)
            image.color = Color.gray;
        else
            image.color = selected
                ? Color.yellow
                : Color.white;
    }

    private void UpdateAppearance()
    {
        if (image == null)
            return;

        if (!IsAlive)
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }

            rectTransform.localPosition = originalPosition;
            image.color = Color.gray;

            return;
        }

        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        damageCoroutine = StartCoroutine(DamageEffect());
    }
    private IEnumerator DamageEffect()
    {
        image.color = Color.red;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Mathf.Sin(elapsed * 60f) * 8f;

            rectTransform.localPosition =
                originalPosition + new Vector2(x, 0f);

            elapsed += Time.deltaTime;

            yield return null;
        }

        rectTransform.localPosition = originalPosition;
        image.color = Color.white;

        damageCoroutine = null;
    }

    
}

