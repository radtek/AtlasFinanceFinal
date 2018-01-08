namespace Falcon.Common.Structures
{
  public sealed class Relation
  {
    public long PersonId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CellNo { get; set; }
    public int RelationTypeId { get; set; }
    public string RelationType { get; set; }
  }
}
