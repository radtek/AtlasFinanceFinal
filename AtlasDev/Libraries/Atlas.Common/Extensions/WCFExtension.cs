/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
 * 
 * 
 *  Description:
 *  ------------------
 *    
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
  *     
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.ServiceModel;


namespace Atlas.Common.Extensions
{
  /// <summary>
  /// WCF Extension methods
  /// </summary>
  public static class WCFExtensions
  {
    /// <summary>
    /// Safe clean-up of WCF proxy client resources, in case of connection abort
    /// </summary>
    /// <typeparam name="T">The WCF proxy class type</typeparam>
    /// <param name="client">WCF proxy client</param>
    /// <param name="work">Action to perform with the client</param>
    public static void Using<T>(this T client, Action<T> work)
        where T : ICommunicationObject
    {
      try
      {
        work(client);
        client.Close();
      }
      catch (CommunicationException)
      {
        client.Abort();
        throw;
      }
      catch (TimeoutException)
      {
        client.Abort();
        throw;
      }
      catch (Exception)
      {
        client.Abort();
        throw;
      }
    }
  }
}
