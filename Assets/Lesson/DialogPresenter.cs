
using System; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum DialoguePresenterState
{
    Await,
    Talk,
    Choice,
    Finish
}

public class DialogPresenter : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private DialogView _dialogueView;
    [SerializeField] private TextAsset[] _dialoguesArr;
    private uint _currentDialogue;
    private DialogNode _currentNode;

    [Header("Events")]
    [SerializeField] private UnityEvent[] OnDialogueFinished;
    public event Action OnDialogueStart;

    [field: Header("States")]
    [field: SerializeField] public bool CanTalk { get; set; }
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
        if (_state == DialoguePresenterState.Finish || !CanTalk)
            return;

        _currentNode = ParseDialogFile.GetDialogTree(_dialoguesArr[_currentDialogue]);

        _dialogueView.SetPresenter(this);
        OnDialogueStart?.Invoke();
        CanTalk = false;
        _dialogueView.StartDialogue(_currentNode.Message, _currentNode.Name);
        _state = DialoguePresenterState.Talk;
        _messagePrinting = true;
    }

    public void NextMessage()
    {
        if (_state != DialoguePresenterState.Talk || _state == DialoguePresenterState.Choice)
            return;

        if (_messagePrinting)
        {
            _dialogueView.InterruptAnimation();
            return;
        }

        if (_currentNode.Children.Count == 0)
        {
            FinishDialogue();
            return;
        }

        GoToChildMessage();
    }

    private void GoToChildMessage()
    {
        if (_currentNode.Children.Count == 1)
        {
            _currentNode = _currentNode.Children[0];
            _dialogueView.NextMessage(_currentNode.Message, _currentNode.Name);
            _messagePrinting = true;
            return;
        }

        List<string> shortNames = new List<string>();
        foreach (var child in _currentNode.Children)
            shortNames.Add(child.ShortName);
        _dialogueView.ActivateButtons(shortNames);
        _state = DialoguePresenterState.Choice;
    }

    private void FinishDialogue()
    {
        _dialogueView.StopDialogue();
        if (_currentNode.ActionId != null)
            OnDialogueFinished[(int)_currentNode.ActionId].Invoke();

        _state = DialoguePresenterState.Await;
        _currentDialogue++;
        if (_currentDialogue > _dialoguesArr.Length - 1)
        {
            _state = DialoguePresenterState.Finish;
        }
    }

    public void SwitchBranch(int index)
    {
        Debug.Log("Button clicked " + index.ToString());
        _currentNode = _currentNode.Children[index];
        _state = DialoguePresenterState.Talk;
        _dialogueView.NextMessage(_currentNode.Message, _currentNode.Name);
        _messagePrinting = true;
    }

    public void Action0()
    {
        Debug.Log("Первая ветка закончена");
    }

    public void Action1()
    {
        Debug.Log("Вторая ветка закончена");
    }
}
