using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.DTO
{
  public sealed class ProvinceDto
  {
    public int ProvinceId { get; set; }
    public Enumerators.General.Province Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.Province>(); }
      set { value = Description.FromStringToEnum<Enumerators.General.Province>(); }
    }
    public string ShortCode { get; set; }
    public string Description { get; set; }
  }
}