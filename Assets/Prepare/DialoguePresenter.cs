using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Xml;

enum DialoguePresenterState
{
    Await,
    Talk,
    Choice,
    Finish
}

public class DialoguePresenter : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private DialogueView _dialogueView;
    [SerializeField] private TextAsset[] _dialoguesArr;
    private uint _currentDialogue;
    private Queue<string> _messageQueue = new Queue<string>();
    private Queue<string> _namesQueue = new Queue<string>();
    private DialogNode _currentNode;

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

        GetData(_dialoguesArr[_currentDialogue]);

        OnDialogueStart?.Invoke();
        CanTalk = false;
        _dialogueView.StartDialogue(_currentNode.Message, _currentNode.Name);
        _state = DialoguePresenterState.Talk;
        _messagePrinting = true;
    }

    private void GetData(TextAsset textAsset)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text); // Загрузите ваш XML файл
        XmlNode rootDialogNode = xmlDoc.SelectSingleNode("/dialog");
        _currentNode = ReadDialogueData.CreateDialogTree(rootDialogNode);
 
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
        if(_currentNode.ActionId != null)
            OnCurrentDialogueFinished[(int)_currentNode.ActionId].Invoke();

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
        Debug.Log("Завершена ветка 3");
    }

    public void Action1()
    {
        Debug.Log("Завершена ветка 4");
    }
}
