using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Online.Web.Validations
{
  interface IClientSideValidationsProvider
  {
    IEnumerable<ClientSideValidation> GetValidations();
  }
}
