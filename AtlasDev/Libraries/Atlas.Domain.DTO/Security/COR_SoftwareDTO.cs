using System;


namespace Atlas.Domain.DTO
{
  [Serializable]
  public class COR_SoftwareDTO
  {
    public Int64 AtlasSoftwareId { get; set; }
    public Enumerators.General.ApplicationIdentifiers AtlasApplication { get; set; }
    public string AppVersion { get; set; }
    public DateTime ReleasedDT { get; set; }
    public string FileHash { get; set; }
    public string Comments { get; set; }    
  }
}
