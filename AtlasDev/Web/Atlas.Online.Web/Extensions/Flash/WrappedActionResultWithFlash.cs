using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Atlas.Online.Web.Extensions.Flash
{
  public class WrappedActionResultWithFlash<TActionResult> : ActionResult where TActionResult : ActionResult
  {
    public WrappedActionResultWithFlash(TActionResult wrappingResult, IDictionary<string, string> flashMessages)
    {
      if (wrappingResult == null)
      {
        throw new ArgumentNullException("wrappingResult");
      }

      if (flashMessages == null)
      {
        throw new ArgumentNullException("flashMessages");
      }

      WrappingResult = wrappingResult;
      FlashMessages = flashMessages;
    }

    public TActionResult WrappingResult { get; private set; }

    public IDictionary<string, string> FlashMessages { get; private set; }

    public override void ExecuteResult(ControllerContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context");
      }

      var storage = new FlashStorage(context.Controller.TempData);

      foreach (var pair in FlashMessages)
      {
        storage.Add(pair.Key, pair.Value);
      }

      WrappingResult.ExecuteResult(context);
    }
  }
}