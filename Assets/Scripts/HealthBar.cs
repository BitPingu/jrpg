using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float _timeToDrain = .25f;
    [SerializeField] private Gradient _healthBarGradient;

    private float _target = 1f;

    private Color _newHealthBarColor;

    private Coroutine drainHealthBarCoroutine;

    private void Start()
    {
        // set health bar color
        GetComponent<Image>().color = _healthBarGradient.Evaluate(_target);

        CheckHealthBarGradientAmount();
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        _target = currentHealth / maxHealth;

        drainHealthBarCoroutine = StartCoroutine(DrainHealthBar());

        CheckHealthBarGradientAmount();
    }

    private IEnumerator DrainHealthBar()
    {
        float fillAmount = GetComponent<Image>().fillAmount;
        Color currentColor = GetComponent<Image>().color;

        float elapsedTime = 0f;
        while (elapsedTime < _timeToDrain)
        {
            elapsedTime += Time.deltaTime;
            // lerp fill amount
            GetComponent<Image>().fillAmount = Mathf.Lerp(fillAmount, _target, (elapsedTime / _timeToDrain));
            // lerp color based on gradient
            GetComponent<Image>().color = Color.Lerp(currentColor, _newHealthBarColor, (elapsedTime / _timeToDrain));
            yield return null;
        }
    }

    private void CheckHealthBarGradientAmount()
    {
        _newHealthBarColor = _healthBarGradient.Evaluate(_target);
    }
}
