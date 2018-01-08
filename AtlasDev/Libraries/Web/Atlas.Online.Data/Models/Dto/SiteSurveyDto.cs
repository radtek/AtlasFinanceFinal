using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Online.Data.Models.DTO
{
  public sealed class SiteSurveyDto
  {
    public int SiteSurveyId { get; set; }
    [Required]
    public string Email { get; set; }
    public ClientDto Client { get; set; }
    [Required]
    public string Name { get; set; }

    [Required]
    public int Q1Option { get; set; }
    [Required]
    public int Q2Option { get; set; }
    [Required]
    public int Q3Option { get; set; }
    [Required]
    public int Q4Option { get; set; }
    
    public string Comments { get; set; }

    public DateTime CreateDate { get; set; }

    public const string VOTED_COOKIE = "_SSVoted";
  }
}
