using System.Collections;
using UnityEngine;
using TMPro;

public static class UIAnimator
{
    //  Nesneyi Aşağıdan Yukarı Kaydırarak Getirme
    public static IEnumerator MoveFromTo(RectTransform rectTransform, Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsed = 0f;
        rectTransform.anchoredPosition = startPos;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); 
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        rectTransform.anchoredPosition = endPos;
    }

    // Pop-up 
    public static IEnumerator PopIn(Transform targetTransform, float duration = 0.25f)
    {
        float elapsed = 0f;
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        targetTransform.localScale = initialScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Zıplama hissi veren matematiksel eğri (Overshoot)
            float bounce = Mathf.Sin(t * Mathf.PI * 0.75f) * 1.15f; 
            targetTransform.localScale = Vector3.one * Mathf.Min(bounce, 1.1f);
            yield return null;
        }

        targetTransform.localScale = targetScale;
    }

    // Daktilo Yazı Efekti
    public static IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText, float letterDelay = 0.03f)
    {
        textComponent.text = "";
        foreach (char c in fullText.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(letterDelay);
        }
    }

    // Buton Tıklama Esnemesi
    public static IEnumerator ButtonClickEffect(Transform buttonTransform)
    {
        Vector3 originalScale = Vector3.one;
        buttonTransform.localScale = originalScale * 0.9f; // Hafif küçül
        yield return new WaitForSeconds(0.08f);
        buttonTransform.localScale = originalScale * 1.05f; // Hafif zıpla
        yield return new WaitForSeconds(0.08f);
        buttonTransform.localScale = originalScale;
    }
}