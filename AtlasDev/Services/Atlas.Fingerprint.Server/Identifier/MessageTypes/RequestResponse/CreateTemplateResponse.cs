using System;


namespace Atlas.FP.Identifier.MessageTypes.RequestResponse
{
  public class CreateTemplateResponse
  {
    public CreateTemplateResponse(byte[] template, byte[] reversedTemplate, string errorMessage)
    {
      Template = template;
      ReversedTemplate = reversedTemplate;
      ErrorMessage = errorMessage;
    }


    public byte[] Template { get; set; }

    public byte[] ReversedTemplate { get; set; }

    public string ErrorMessage { get; set; }

  }

}
