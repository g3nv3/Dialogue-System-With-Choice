using System.Collections;
using TMPro;
using UnityEngine;
using Prepare;
using System;

public class DialogueView : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Animator _animator;
    public event Action OnFinishMessage;
    private bool _isDialogueRun;

    [Header("Animation")]
    [SerializeField] private float betweenHalf = 0.05f;
    [SerializeField] private float betweenChar = 0.03f;
    [SerializeField] private float smoothTime = 0.1f;
    private TextAnimator _textAnimator;

    private void Awake()
    {
        _textAnimator = new TextAnimator(_messageText, betweenHalf, betweenChar, smoothTime);
    }

    private void Update()
    {
        if (_isDialogueRun)
            _textAnimator.TextAnimatorUpdate();
    }
    public void StartDialogue(string message, string name)
    {
        _isDialogueRun = true;
        _nameText.text = name;
        NewMessage(message);
        _animator.SetTrigger("Start");
    }
    public void InterruptAnimation()
    {
        _textAnimator.InterruptAnimation();
    }

    public void StopDialogue()
    {
        _isDialogueRun = false;
        StartCoroutine(Wait());
        _animator.SetTrigger("Stop");
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);
        HideDialogue();
    }

    public void NextMessage(string message, string name)
    {
        _nameText.text = name;
        NewMessage(message);
    }

    private void NewMessage(string text)
    {
        _messageText.text = text;
        _messageText.ForceMeshUpdate();
        _textAnimator.StartAnimation(text);
    }


    private void HideDialogue()
    {
        _textAnimator.HideText();
        _nameText.text = "";
    }

    private void FinishMessage()
    {
        OnFinishMessage?.Invoke();
    }

    private void OnEnable()
    {
        _textAnimator.OnAnimationFinished += FinishMessage;
    }

    private void OnDisable()
    {
        _textAnimator.OnAnimationFinished -= FinishMessage;
    }

}
