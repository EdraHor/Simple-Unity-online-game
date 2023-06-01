using ProtoBuf;

public enum AnswerType
{
    AutorizationStatus, GetClientsData
}

/// <summary>
/// Содержин ответ на запрос от клиентов
/// </summary>
[ProtoContract]
public class Answer
{
    [ProtoMember(1)] public AnswerType AnswerType { get; }
    [ProtoMember(2)] public bool AnswerValue;

    public Answer(AnswerType answerType, bool answer)
    {
        this.AnswerType = answerType;
        this.AnswerValue = answer;
    }

    public Answer() { }
}