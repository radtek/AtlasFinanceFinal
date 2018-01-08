using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Web.Service.Entities.Verification
{
  public sealed class Questions
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

    public long StoreId { get; set; }
    public long QuestionId { get; set; }
    public string Question { get; set; }
    public List<Answer> Answers { get; set; }
  }
}