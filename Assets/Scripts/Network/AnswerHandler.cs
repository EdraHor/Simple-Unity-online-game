using System;

namespace Network
{
    public class AnswerHandler
    {
        public event Action<bool> OnAutorizationAnswer;

        public void OnAnswerReceive(Answer answer)
        {
            switch (answer.AnswerType)
            {
                case AnswerType.AutorizationStatus:
                    OnAutorizationAnswer?.Invoke(answer.AnswerValue);
                    break;
                default:
                    break;
            }
        }
    }
}
