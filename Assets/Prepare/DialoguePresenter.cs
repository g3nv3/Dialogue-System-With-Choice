using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

enum DialoguePresenterState
{
    Await,
    Talk,
    Finish
}

public class DialoguePresenter : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private DialogueView _dialogueView;
    [SerializeField] private DialogueData[] _dialoguesArr;
    private uint _currentDialogue;
    private Queue<string> _messageQueue = new Queue<string>();
    private Queue<string> _namesQueue = new Queue<string>();

    [Header("Events")]
    [SerializeField] private UnityEvent[] OnCurrentDialogueFinished;
    public event Action OnDialogueStart;

    [field: Header("States")]
    [field: SerializeField] public bool CanTalk { get; set; }
    [SerializeField] private bool _playerInArea;
    [SerializeField] private DialoguePresenterState _state;
    private bool _messagePrinting;

    private void OnEnable()
    {
        _dialogueView.OnFinishMessage += StopPrinting;
    }

    private void OnDisable()
    {
        _dialogueView.OnFinishMessage -= StopPrinting;
    }

    private void StopPrinting()
    {
        _messagePrinting = false;
    }
    public void StartDialogue()
    {
        if (_state == DialoguePresenterState.Finish || !_playerInArea || !CanTalk)
            return;

        for(int i = 0; i < _dialoguesArr[_currentDialogue].Messages.Length; i++)
        {
            _messageQueue.Enqueue(_dialoguesArr[_currentDialogue].Messages[i]);
            _namesQueue.Enqueue(_dialoguesArr[_currentDialogue].Names[i]);
        }

        OnDialogueStart?.Invoke();
        CanTalk = false;
        _dialogueView.StartDialogue(_messageQueue.Dequeue(), _namesQueue.Dequeue());
        _state = DialoguePresenterState.Talk;
        _messagePrinting = true;
    }
    public void NextMessage()
    {
        if (_state != DialoguePresenterState.Talk)
            return;

        if (_messagePrinting)
        {
            _dialogueView.InterruptAnimation();
            return;
        }

        if (_messageQueue.Count > 0)
        {
            _dialogueView.NextMessage(_messageQueue.Dequeue(), _namesQueue.Dequeue());
            _messagePrinting = true;
        }
        else
            FinishDialogue();
    }

    private void FinishDialogue()
    {
        _dialogueView.StopDialogue();
        if (_currentDialogue <= OnCurrentDialogueFinished.Length
                && OnCurrentDialogueFinished.Length > 0)
            OnCurrentDialogueFinished[_currentDialogue].Invoke();

        _state = DialoguePresenterState.Await;
        _currentDialogue++;
        if (_currentDialogue > _dialoguesArr.Length - 1)
        {
            _state = DialoguePresenterState.Finish;
        }
    }
}
