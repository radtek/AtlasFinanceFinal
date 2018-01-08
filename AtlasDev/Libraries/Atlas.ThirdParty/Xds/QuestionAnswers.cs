using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.ThirdParty.Xds
{
  public sealed class QuestionAnswers
  {
    public sealed class Answer
    {
      public long AnswerId { get; set; }
      public string AnswerDescription { get; set; }
      public bool IsAnswer { get; set; }

      public Answer()
      {
        this.IsAnswer = false;
      }
    }

    public long AuthenticationProcessStoreId { get; set; }
    public long QuestionId { get; set; }
    public string Question { get; set; }
    public List<Answer> Answers { get; set; }
  } 
}
