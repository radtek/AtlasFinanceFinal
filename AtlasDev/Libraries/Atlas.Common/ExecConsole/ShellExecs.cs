/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
 * 
 * 
 *  Description:
 *  ------------------
 *    Utilities for executing processes
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;


namespace Atlas.Common
{ 
  public class ShellExecs
  {
    #region Public static functions

    public static bool ExecProcessNoWin(string fileName, string arguments, int timeoutMS,
      out string errorMessage, out string outputMessage)
    {
      errorMessage = null;
      outputMessage = null;

      bool result = false;

      try
      {
        using (var proc = new Process())
        {
          proc.StartInfo.FileName = fileName;
          proc.StartInfo.Arguments = arguments;

          proc.StartInfo.RedirectStandardError = true;
          proc.StartInfo.RedirectStandardOutput = true;
          proc.StartInfo.UseShellExecute = false;
          proc.EnableRaisingEvents = false;
          proc.StartInfo.CreateNoWindow = true;

          proc.Start();
          result = proc.WaitForExit(timeoutMS);

          errorMessage = proc.StandardError.ReadToEnd();
          proc.WaitForExit();

          outputMessage = proc.StandardOutput.ReadToEnd();
          proc.WaitForExit();
        }
      }
      catch (Exception err)
      {
        errorMessage = err.Message;
        return false;
      }

      return result;
    }


    /// <summary>  
    /// Execute a shell command - Generic (simulate running from cmd)
    /// </summary>  
    /// <param name="fileToExecute">File/Command to execute</param>  
    /// <param name="cmdLineParams">Command line parameters to pass</param>   
    /// <param name="asyncInd">Indicate if process must wait for task to complete</param>   
    /// <param name="outputMessage">returned string value after executing shell command</param>   
    /// <param name="outputError">Error messages generated during shell execution</param>   
    public static void ExecShellCommand(string fileToExecute, string cmdLineParams, bool asyncInd, ref string outputMessage, ref string outputError)
    {
      // Set process variable  
      // Provides access to local and remote processes and enables you to start and stop local system processes.  

      outputMessage = "";
      outputError = "";

      var process = (Process)null;
      try
      {
        process = new Process();

        // invokes the cmd process specifying the command to be executed.  
        var cmdProcess = string.Format(CultureInfo.InvariantCulture, @"{0}\cmd.exe", new object[] { Environment.SystemDirectory });


        // pass executing file to cmd (Windows command interpreter) as a arguments  
        // /C tells cmd that we want it to execute the command that follows, and then exit.  
        var arguments = string.Format(CultureInfo.InvariantCulture, "/C {0}", new object[] { fileToExecute });

        // pass any command line parameters for execution  
        if (cmdLineParams != null && cmdLineParams.Length > 0)
        {
          arguments += string.Format(CultureInfo.InvariantCulture, " {0}", new object[] { cmdLineParams, CultureInfo.InvariantCulture });
        }

        // Specifies a set of values used when starting a process.  
        var processStartInfo = new ProcessStartInfo(cmdProcess, arguments)
        {
          /* sets a value indicating not to start the process in a new window.   */
          CreateNoWindow = true,
          /* sets a value indicating not to use the operating system shell to start the process.   */
          UseShellExecute = false,
          /* sets a value that indicates the output/input/error of an application is written to the Process.  */
          RedirectStandardOutput = true,
          RedirectStandardInput = true,
          RedirectStandardError = true
        };
        process.StartInfo = processStartInfo;

        // Starts a process resource and associates it with a Process component.  
        process.Start();

        if (asyncInd == false)
        {
          // Instructs the Process component to wait indefinitely for the associated process to exit.  
          outputError = process.StandardError.ReadToEnd();
          process.WaitForExit();

          // Instructs the Process component to wait indefinitely for the associated process to exit.  
          outputMessage = process.StandardOutput.ReadToEnd();
          process.WaitForExit();
        }

      }
      catch (Win32Exception _Win32Exception)
      {
        outputError = "Win32 Exception caught in process: " + _Win32Exception;
      }
      catch (Exception _Exception)
      {
        outputError = "Exception caught in process: " + _Exception;
      }
      finally
      {
        // close process and do cleanup  
        if (process != null)
        {
          process.Close();
          process.Dispose();
          process = null;
        }
      }
    }


    /// <summary>  
    /// Execute a shell command  -  Generic (simulate running from run)
    /// </summary>  
    /// <param name="fileToExecute">File/Command to execute</param>  
    /// <param name="cmdLineParams">Command line parameters to pass</param>   
    /// <param name="runAsync">Indicate if process must wait for task to complete</param>   
    /// <param name="outputError">Error messages generated during shell execution</param>   
    public static void ExecRunCmd(string fileToExecute, string cmdLineParams, bool runAsync, ref string outputError)
    {
      Process proc = null;
      outputError = "";
      try
      {
        proc = new Process();
        proc.EnableRaisingEvents = false;
        proc.StartInfo.FileName = fileToExecute;
        proc.StartInfo.Arguments = cmdLineParams;
        proc.Start();

        if (runAsync == true)
        {
          proc.WaitForExit();
        }
      }
      catch (Exception _Exception)
      {
        outputError = "Exception caught in process: " + _Exception;
      }
      finally
      {
        // close process and do cleanup  
        proc.Close();
        proc.Dispose();
        proc = null;
      }
    }

    #endregion
  }
}
