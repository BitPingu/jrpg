using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] private float _timeToDrain = .25f;
    [SerializeField] private Gradient _barGradient;

    private float _target = 1f;

    private Color _newBarColor;

    private Coroutine changeBarCoroutine;

    private void Start()
    {
        // set bar color
        GetComponent<Image>().color = _barGradient.Evaluate(_target);

        CheckBarGradientAmount();
    }

    public void UpdateBar(float maxAmount, float currentAmount)
    {
        _target = currentAmount / maxAmount;

        changeBarCoroutine = StartCoroutine(ChangeBar());

        CheckBarGradientAmount();
    }

    private IEnumerator ChangeBar()
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
            GetComponent<Image>().color = Color.Lerp(currentColor, _newBarColor, (elapsedTime / _timeToDrain));
            yield return null;
        }
    }

    private void CheckBarGradientAmount()
    {
        _newBarColor = _barGradient.Evaluate(_target);
    }
}
