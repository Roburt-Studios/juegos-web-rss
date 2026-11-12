using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasFader : MonoBehaviour
{
    [SerializeField, Min(0.01f)]
    private float defaultDuration = 0.35f;

    [SerializeField]
    private bool fadeInOnStart;

    private CanvasGroup canvasGroup;
    private readonly Queue<FadeRequest> fadeQueue = new Queue<FadeRequest>();
    private Coroutine queueCoroutine;

    private struct FadeRequest
    {
        public float targetAlpha;
        public float duration;

        public FadeRequest(float targetAlpha, float duration)
        {
            this.targetAlpha = targetAlpha;
            this.duration = duration;
        }
    }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (fadeInOnStart)
        {
            canvasGroup.alpha = 0f;
            FadeIn();
        }
    }

    public void FadeIn()
    {
        FadeTo(1f, defaultDuration);
    }

    public void FadeOut()
    {
        FadeTo(0f, defaultDuration);
    }

    public void FadeIn(float duration)
    {
        FadeTo(1f, duration);
    }

    public void FadeOut(float duration)
    {
        FadeTo(0f, duration);
    }

    public void FadeTo(float targetAlpha, float duration)
    {
        fadeQueue.Enqueue(new FadeRequest(Mathf.Clamp01(targetAlpha), Mathf.Max(0f, duration)));

        if (queueCoroutine == null)
            queueCoroutine = StartCoroutine(ProcessQueueRoutine());
    }

    private IEnumerator ProcessQueueRoutine()
    {
        while (fadeQueue.Count > 0)
        {
            FadeRequest request = fadeQueue.Dequeue();
            yield return FadeRoutine(request.targetAlpha, request.duration);
        }

        queueCoroutine = null;
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;

        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
        }
        else
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }

        bool visible = targetAlpha > 0f;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;
    }
}
