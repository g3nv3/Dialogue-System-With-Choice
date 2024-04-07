using DG.Tweening;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class TextAnimator
{
    public event Action OnAnimationFinished;

    [SerializeField] private TextMeshProUGUI _message;
    [SerializeField] private float _betweenHalf = 0.05f;
    [SerializeField] private float _betweenChar = 0.03f;
    [SerializeField] private float _smoothTime = 0.1f;

    private List<float> _leftAlphas;
    private List<float> _rightAlphas;

    private bool _isAnimating = false;

    public TextAnimator(TextMeshProUGUI messageText, float bh = 0.05f,
      float bC = 0.03f, float sT = 0.1f)
    {
        _message = messageText;
        _betweenChar = bC;
        _betweenHalf = bh;
        _smoothTime = sT;

        _leftAlphas = new float[_message.text.Length].ToList();
        _rightAlphas = new float[_message.text.Length].ToList();
    }

    public void StartAnimation(string text)
    {
        _message.text = text;
        _isAnimating = true;
        ResetVertexInfo();
        Coroutines.StartRoutine(Smooth(0));
    }

    public void InterruptAnimation()
    {
        if (_isAnimating)
        {
            OnAnimationFinished?.Invoke();
            _isAnimating = false;
            Visible(true);
        }
    }

    public void TextAnimatorUpdate()
    {
        if (_isAnimating)
            SwitchColor();
        CheckAnimationFinished();
    }

    public void HideText()
    {
        Visible(false);
    }

    private void CheckAnimationFinished()
    {
        if (_rightAlphas[_rightAlphas.Count - 1] == 255 && _isAnimating)
        {
            OnAnimationFinished?.Invoke();
            _isAnimating = false;
        }
    }

    private void ResetVertexInfo()
    {
        Visible(false);
        _leftAlphas.Clear();
        _rightAlphas.Clear();
        _message.ForceMeshUpdate();
        _leftAlphas = new float[_message.text.Length].ToList();
        _rightAlphas = new float[_message.text.Length].ToList();
    }

    private void Visible(bool visible)
    {
        Coroutines.StopAllRoutine();
        DOTween.Kill(1);

        for (int i = 0; i < _leftAlphas.Count; i++)
        {
            _leftAlphas[i] = visible ? 255 : 0;
            _rightAlphas[i] = visible ? 255 : 0;
        }
        SwitchColor();
    }

    private void SwitchColor()
    {
        for (int i = 0; i < _leftAlphas.Count; i++)
        {
            if (_message.textInfo.characterInfo[i].character != '\n' &&
                _message.textInfo.characterInfo[i].character != ' ')
            {
                int meshIndex = _message.textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = _message.textInfo.characterInfo[i].vertexIndex;

                Color32[] vertexColors = _message.textInfo.meshInfo[meshIndex].colors32;

                vertexColors[vertexIndex + 0].a = (byte)_leftAlphas[i];
                vertexColors[vertexIndex + 1].a = (byte)_leftAlphas[i];
                vertexColors[vertexIndex + 2].a = (byte)_rightAlphas[i];
                vertexColors[vertexIndex + 3].a = (byte)_rightAlphas[i];

            }
        }
        _message.UpdateVertexData();
    }

    private IEnumerator Smooth(int i)
    {
        if (i >= _leftAlphas.Count)
            yield break;

        DOTween.To(
            () => _leftAlphas[i],
            x => _leftAlphas[i] = x,
            255,
            _smoothTime)
        .SetEase(Ease.Linear)
        .SetId(1);

        yield return new WaitForSeconds(_betweenHalf);

        DOTween.To(
            () => _rightAlphas[i],
            x => _rightAlphas[i] = x,
            255,
                _smoothTime)
            .SetEase(Ease.Linear)
            .SetId(1);

        yield return new WaitForSeconds(_betweenChar);
        Coroutines.StartRoutine(Smooth(i + 1));
    }
}

