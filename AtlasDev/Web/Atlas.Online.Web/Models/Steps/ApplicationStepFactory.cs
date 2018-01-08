using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Common.Mappers;
using Atlas.Online.Web.Models.Steps;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlas.Online.Web.Models
{

  public class ApplicationStepFactory
  {
    public enum Direction
    {
      None,
      Forward,
      Backward
    }    

    public static Type[] StepTypes 
    {
      get
      {
        return new Type[] {
          typeof(PersonalDetailsStep), // Step 1
          typeof(EmployerDetailsStep), // Step 2
          typeof(IncomeExpensesStep),  // Step 3
          typeof(ConfirmVerifyStep),   // Step 4
          typeof(VerifyStep),          // Step 5
          typeof(QuoteAcceptance),     // Step 6
        };
      }
    }

    public static Type GetStepTypeById(int stepId)
    {
      Type stepType = null;

      if (!StepTypes.TryGetValue(stepId - 1, out stepType))
      {
        throw new InvalidOperationException(String.Format("Invalid Step {0}.", stepId));
      }

      return stepType;
    }

    public static ApplicationStepBase CreateFirst()
    {
      return Create(1);
    }

    public static ApplicationStepBase Create(int stepId, Direction direction = Direction.None)
    {
      Type stepType = GetStepTypeById(GetStepId(stepId, direction));
      return (ApplicationStepBase)Activator.CreateInstance(stepType);
    }

    public static ApplicationStepBase Create(int stepId, Application application, Direction direction = Direction.None)
    {
      if (application == null)
      {
        return CreateFirst();
      }

      var obj = Create(stepId, direction);
      obj.ApplicationId = application.ApplicationId;
      obj.Populate(application);
      return obj;
    }

    public static ApplicationStepBase Create(Application application, Direction direction = Direction.None)
    {
      if (application == null)
      {
        return CreateFirst();
      }

      var obj = Create(application.Step, direction);
      obj.ApplicationId = application.ApplicationId;
      obj.Populate(application);
      return obj;
    }

    public static ApplicationStepBase Create(int stepId, JToken data)
    {
      Type stepType = GetStepTypeById(stepId);
      return (ApplicationStepBase)JMapper.Map(stepType, data);
    }

    public static ApplicationStepBase Create(Type type, Application application)
    {
      if (!type.IsSubclassOf(typeof(ApplicationStepBase)))
      {
        throw new InvalidOperationException(String.Format("{0} is not a subclass of ApplicationStepBase.", type));
      }

      ApplicationStepBase instance = (ApplicationStepBase)Activator.CreateInstance(type);
      instance.ApplicationId = application.ApplicationId;
      instance.Populate(application);
      return instance;
    }

    public static int GetStepId(int stepId, Direction direction = Direction.None)
    {
      switch (direction)
      {        
        case Direction.Forward:
          return Math.Min(StepTypes.Count(), stepId + 1);
        case Direction.Backward:
          return Math.Max(1, stepId - 1);
        case Direction.None:
        default:        
          return stepId;
      }
    }

    public static int GetStepIdByType(Type type)
    {
      if (!type.IsSubclassOf(typeof(ApplicationStepBase)))
      {
        throw new InvalidOperationException(String.Format("{0} is not a subclass type of ApplicationStepBase.", type.Name));
      }

      return Array.FindIndex(StepTypes, x => x.Equals(type)) + 1; 
    }

  }
}