using Atlas.Ass.Framework.Repository;
using Atlas.Ass.Repository.Properties;
using Atlas.Common.Utils;
using Falcon.Common.Interfaces.Services;

namespace Atlas.Ass.Repository
{
  public class AssBureauRepository : IAssBureauRepository
  {
    private readonly IConfigService _configService;

    public AssBureauRepository(IConfigService configService)
    {
      _configService = configService;
    }

    public bool DoesNlrExistsInAss(string nlrReference)
    {
      var result = false;
      var obj = GetScalar(Resources.QRY_NLR_CheckIfReferenceExists, nlrReference);
      if (obj != null && bool.TryParse(obj.ToString(), out result))
        return result;
      return result;
    }

    private object GetScalar(string templateQuery, params object[] parameters)
    {
      var query = string.Format(templateQuery, parameters);
      var queryUtil = new RawSql();
      var obj = queryUtil.ExecuteScalar(query, _configService.AssConnection);
      return obj;
    }
  }
}
