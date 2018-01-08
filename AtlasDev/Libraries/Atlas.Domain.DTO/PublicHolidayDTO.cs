using System;


namespace Atlas.Domain.DTO
{
  public sealed class PublicHolidayDTO
  {
    public int PublicHolidayId { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public int Year { get; set; }
  }
}
