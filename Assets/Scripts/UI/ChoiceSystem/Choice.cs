using System;

[Serializable]
public class Choice
{
    public string ChoiceText { get; private set; }
    public Action OnSelected;

    public Choice(string choiceText) => ChoiceText = choiceText;
}