
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogView : MonoBehaviour
{
    public event Action OnFinishMessage;
    private bool _isDialogueRun;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private GameObject[] _buttons;
    [SerializeField] private TextMeshProUGUI[] _buttonsText;
    [SerializeField] private Animator _animator;
    private TextAnimator _textAnimator;
    private DialogPresenter _dialoguePresenter;


    [Header("Animation")]
    [SerializeField] private float betweenHalf = 0.05f;
    [SerializeField] private float betweenChar = 0.03f;
    [SerializeField] private float smoothTime = 0.1f;

    private void Awake()
    {
        _textAnimator = new TextAnimator(_messageText, betweenHalf, betweenChar, smoothTime);
    }

    private void Update()
    {
        if (_isDialogueRun)
            _textAnimator.TextAnimatorUpdate();
    }

    public void SetPresenter(DialogPresenter presenter)
    {
        _dialoguePresenter = presenter;
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
        HideDialog();
    }

    private void HideDialog()
    {
        _textAnimator.HideText();
        _nameText.text = "";
    }

    public void NextMessage(string message, string name)
    {
        _nameText.text = name;
        NewMessage(message);
    }

    private void NewMessage(string text)
    {
        _messageText.text = text;
        _textAnimator.StartAnimation(text);
    }

    public void HideButtons()
    {
        foreach (var button in _buttons)
            button.SetActive(false);
    }
    public void ActivateButtons(List<string> shortNames)
    {
        for (int i = 0; i < shortNames.Count; i++)
        {
            _buttons[i].SetActive(true);
            _buttonsText[i].text = shortNames[i];
        }
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

    public void ClickOnButtonChoice(int indexButton)
    {
        HideButtons();
        _dialoguePresenter.SwitchBranch(indexButton);
    }
}
