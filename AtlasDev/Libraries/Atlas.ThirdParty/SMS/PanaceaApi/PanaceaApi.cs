using System;
using System.IO;
using System.Collections;
using System.Web;
using System.Net;
using System.Text;

namespace Atlas.ThirdParty.PanaceaMobile
{

  public class PanaceaApi
  {
    private String apiUrl = "http://bli.panaceamobile.com/json";
    private String username = "";
    private String password = "";
    private String encodingScheme = "UTF-8";
    private Boolean debug = false;
    private Boolean performActionsImmediately = true;
    private ArrayList queuedActions = new ArrayList();

    public PanaceaApi(String username, String password)
    {
      this.username = username;
      this.password = password;
    }

    public void setUsername(String username)
    {
      this.username = username;
    }

    public void setPassword(String password)
    {
      this.password = password;
    }

    public void setDebugging(Boolean state)
    {
      this.debug = state;
    }

    public void setPerformActionsImmediately(Boolean state)
    {
      this.performActionsImmediately = state;
    }

    public void setEncodingScheme(String scheme)
    {
      this.encodingScheme = scheme;
    }

    private void displayDebug(String data, Boolean force = false)
    {
      if (this.debug || force)
      {
        DateTime now = DateTime.Now;
        String timestamp = now.Year + "-" + now.Month + "-" + now.Day + " " + now.Hour + ":" + now.Minute + ":" + now.Second;
        System.Console.WriteLine(timestamp + ": " + data);
      }
    }

    public Boolean validateResult(Hashtable json)
    {
      if (json != null)
      {
        try
        {
          int status = -1;

          if (json.ContainsKey("status"))
          {
            status = Convert.ToInt32(json["status"]);
            if (status >= 0)
            {
              // OK! //
              return true;
            }
          }
          displayDebug("Validation Result: " + status.ToString() + ", Result: " + json);
        }
        catch (Exception e)
        {
          displayDebug("Source: " + e.Source + ", Message: " + e.Message + ", Trace: " + e.StackTrace, true);
        }
      }

      return false;
    }

    private Hashtable call_api_method(Hashtable map)
    {
      try
      {
        if (this.performActionsImmediately == true)
        {

          WebClient client = new WebClient();
          client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR1.0.3705;)");
          // client.Headers.Add("Connection", "keep-alive");

          client.QueryString.Add("username", Uri.EscapeDataString(this.username));
          client.QueryString.Add("password", Uri.EscapeDataString(this.password));

          IDictionaryEnumerator e = map.GetEnumerator();
          // e.
          while (e.MoveNext())
          {
            if (e.Value.ToString() != "" && e.Value.ToString() != "0")
            {
              client.QueryString.Add(e.Key.ToString(), Uri.EscapeDataString(e.Value.ToString()));
              // System.Console.WriteLine(e.Key.ToString() + "=" + Uri.EscapeDataString(e.Value.ToString()));
            }
          }

          Stream data = client.OpenRead(this.apiUrl);
          StreamReader reader = new StreamReader(data);
          string response = (String)reader.ReadToEnd();
          reader.Close();
          data.Close();

          return (Hashtable)Procurios.Public.JSON.JsonDecode(response);

        }
        else
        {
          if (this.username == "" || this.password == "")
          {
            System.Console.WriteLine("You need to specify your username and password using setUsername() and setPassword() to perform bulk actions");
            //return FALSE;
          }

          Hashtable command = new Hashtable();
          Hashtable parameters = new Hashtable();

          IDictionaryEnumerator e = map.GetEnumerator();
          while (e.MoveNext())
          {
            if (e.Value.ToString() != "" && e.Value.ToString() != "0" && e.Key.ToString() != "action")
            {
              parameters.Add(e.Key.ToString(), Uri.EscapeDataString(e.Value.ToString()));
            }
          }

          command.Add("command", map["action"]);
          command.Add("params", parameters);

          this.queuedActions.Add(command);

          return null;
        }
      }
      catch (WebException e)
      {
        displayDebug("Source: " + e.Source + ", Message: " + e.Message + ", Trace: " + e.StackTrace, true);
      }
      catch (Exception e)
      {
        displayDebug("Source: " + e.Source + ", Message: " + e.Message + ", Trace: " + e.StackTrace, true);
      }
      return null;
    }

    public Hashtable address_book_contacts_get_list(int group_id)
    {
      /* Result: {"status":0,"message":"OK","details":[{"id":"1","phone_number":"98463673","first_name":"Bob","last_name":"Fisher"}]}
       * 
       * Hashtable [
       *   "status"
       *   "message"
       *   ArrayList "details" [0..n
       *     Hashtable [
       *       "id"
       *       "phone_number"
       *       "first_name"
       *       "last_name"
       *     ]
       *   ]
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "address_book_contacts_get_list");
      map.Add("group_id", group_id);
      return call_api_method(map);
      // return (ArrayList)"details"
    }

    public Hashtable address_book_contact_add(int group_id, String phone_number, String first_name = "", String last_name = "")
    {
      /* Result: {"status":0,"message":"OK","details":"16"}
       * 
       * Hashtable [
       *   "status",
       *   "message",
       *   "details"
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "address_book_contact_add");
      map.Add("group_id", group_id);
      map.Add("phone_number", phone_number);
      map.Add("first_name", first_name);
      map.Add("last_name", last_name);
      return call_api_method(map);
      // return (int)"details"
    }

    public Hashtable address_book_contact_delete(int contact_id)
    {
      /* Result:
       * 
       * Hashtable [
       *   "status",
       *   "message",
       *   "details"
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "address_book_contact_delete");
      map.Add("contact_id", contact_id);
      return call_api_method(map);
      // return ? not sure the result of this? probably true / false
    }

    public Hashtable address_book_contact_update(int contact_id, String phone_number = "", String first_name = "", String last_name = "")
    {
      /* Result: {"status":0,"message":"OK"}
       * 
       * Hashtable [
       *   "status",
       *   "message"
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "address_book_contact_update");
      map.Add("contact_id", contact_id);
      map.Add("phone_number", phone_number);
      map.Add("first_name", first_name);
      map.Add("last_name", last_name);
      return call_api_method(map);
      // return "status" >= 0 ? true : false
    }

    public Hashtable address_book_groups_get_list()
    {
      /* Result: {"status":0,"message":"OK","details":[{"id":"1","name":"My Friends"}]}
       * 
       * Hashtable [
       *   "status"
       *   "message"
       *   ArrayList "details" [0..n
       *     Hashtable [
       *       "id"
       *       "name"
       *     ]
       *   ]
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "address_book_groups_get_list");
      return call_api_method(map);
      // return (ArrayList)"details"
    }

    public Hashtable address_book_group_add(String name)
    {
      /* Result: {"status":0,"message":"OK","details":"7"}
       * 
       * Hashtable [
       *   "status",
       *   "message",
       *   "details"
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "address_book_group_add");
      map.Add("name", name);
      return call_api_method(map);
      // return (int)"details"
    }

    public Hashtable address_book_group_delete(int group_id)
    {
      /* Result: {"status":0,"message":"OK"}
       * 
       * Hashtable [
       *   "status",
       *   "message"
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "address_book_group_delete");
      map.Add("group_id", group_id);
      return call_api_method(map);
      // return (boolean)"status" >= 0 ? true : false        
    }

    public Hashtable batches_list()
    {
      /* Result: {"status":0,"message":"OK","details":[{"id":"11","name":"test","status":32,"deletable":false},{"id":"12","name":"Relationships","status":32,"deletable":false}]}
       *
       * Hashtable [
       *   "status",
       *   "message",
       *   ArrayList [0..n
       *     Hashtable [
       *       "id",
       *       "name",
       *       "status",
       *       "deletable"
       *     ]
       *   ]
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "batches_list");
      return call_api_method(map);
      // return (ArrayList)"details"
    }

    public Hashtable batch_check_status(int batch_id)
    {
      /* Result: {"status":0,"message":"OK","details":{"status":"64","num_messages":"4","num_processed":"4","num_delivered":"0","num_failed":"0","num_blocked":"0"}}
       * 
       * Hashtable [
       *   "status",
       *   "message"
       *   Hashtable "details" [
       *     "status",
       *     "num_messages",
       *     "num_processed",
       *     "num_delivered",
       *     "num_failed",
       *     "num_blocked"
       *   ]
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "batch_check_status");
      map.Add("batch_id", batch_id);
      return call_api_method(map);
      // return (Hashtable)"details"                
    }

    public Hashtable batch_create(String data, String name, int throughput = 0, Boolean filter = false, String file_type = "csv")
    {
      /* Result: {"status":0,"message":"OK","details":"38"}
       * 
       * Hashtable [
       *   "status",
       *   "message",
       *   "details"
       * ]
       */

      try
      {

        String url = this.apiUrl +
            "?" +
            "action=batch_create" +
            "&" +
            "username" + "=" + Uri.EscapeDataString(this.username) +
            "&" +
            "password" + "=" + Uri.EscapeDataString(this.password) +
            "&" +
            "name" + "=" + Uri.EscapeDataString(name) +
            "&" +
            "throughput" + "=" + Uri.EscapeDataString(throughput.ToString()) +
            "&" +
            "filter" + "=" + Uri.EscapeDataString(filter.ToString().ToLower()) +
            "&" +
            "file_type" + "=" + Uri.EscapeDataString(file_type);

        byte[] buffer = Encoding.ASCII.GetBytes("data=" + Uri.EscapeDataString(data));

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = buffer.Length;
        request.KeepAlive = true;

        Stream PostData = request.GetRequestStream();
        PostData.Write(buffer, 0, buffer.Length);
        PostData.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream responseStream = response.GetResponseStream();
        StreamReader responseStreamReader = new StreamReader(responseStream);

        return (Hashtable)Procurios.Public.JSON.JsonDecode(responseStreamReader.ReadToEnd());
      }
      catch (Exception e)
      {
        displayDebug("Source: " + e.Source + ", Message: " + e.Message + ", Trace: " + e.StackTrace, true);
      }
      return null;
    }

    public Hashtable batch_start(int batch_id)
    {
      /* Result: {"status":0,"message":"OK"}
       *
       * Hashtable [
       *   "status",
       *   "message"
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "batch_start");
      map.Add("batch_id", batch_id);
      return call_api_method(map);
      // return (Boolean) "status" >= 0 ? true : false
    }

    public Hashtable batch_stop(int batch_id)
    {
      /* Result: {"status":0,"message":"OK"}
       *
       * Hashtable [
       *   "status",
       *   "message"
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "batch_stop");
      map.Add("batch_id", batch_id);
      return call_api_method(map);
      // return (Boolean) "status" >= 0 ? true : false
    }

    public Hashtable execute_multiple()
    {
      /* Result: {"status":0,"message":"OK","details":[{"status":1,"message":"Sent","details":"ee83206d-7794-41f3-0109-123000000003"},{"status":1,"message":"Sent","details":"f5d61318-5b4b-419a-0100-123000000003"},{"status":1,"message":"Sent","details":"c9c7b088-72d2-49cf-0101-123000000003"}]}
       * 
       * Hashtable [
       *   "status",
       *   "message",
       *   ArrayList "details" [0..n
       *     Hashtable [
       *       "status",
       *       "message",
       *       String / Int / Double / Hashtable / ArrayList "details" []
       *     ]
       *   ]
       * ]
       */

      try
      {

        String url = this.apiUrl +
            "?" +
            "action=execute_multiple" +
            "&" +
            "username" + "=" + Uri.EscapeDataString(this.username) +
            "&" +
            "password" + "=" + Uri.EscapeDataString(this.password);

        String json = Procurios.Public.JSON.JsonEncode(this.queuedActions);
        byte[] buffer = Encoding.ASCII.GetBytes("data=" + json);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = buffer.Length;
        request.KeepAlive = true;

        Stream PostData = request.GetRequestStream();
        PostData.Write(buffer, 0, buffer.Length);
        PostData.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream responseStream = response.GetResponseStream();
        StreamReader responseStreamReader = new StreamReader(responseStream);

        return (Hashtable)Procurios.Public.JSON.JsonDecode(responseStreamReader.ReadToEnd());
      }
      catch (Exception e)
      {
        displayDebug("Source: " + e.Source + ", Message: " + e.Message + ", Trace: " + e.StackTrace, true);
      }
      return null;
    }

    public Hashtable list_actions()
    {
      Hashtable map = new Hashtable();
      map.Add("action", "list_actions");
      return call_api_method(map);
      // (ArrayList)
    }

    // Note: You NEED to specify the from field unless you have set
    //       up a global value in the console
    public Hashtable message_send(String to, String text, String from = "", int report_mask = 0, String report_url = "", String charset = "", int data_coding = 0)
    {
      /* Result: {"status":1,"message":"Sent","details":"8beda1a8-5c12-489f-0107-123000000003"}
       *
       * Hashtable [
       *   "status",
       *   "message",
       *   "details"
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "message_send");
      map.Add("to", to);
      map.Add("text", text);
      map.Add("from", from);
      map.Add("report_mask", report_mask);
      map.Add("report_url", report_url);
      map.Add("charset", charset);
      map.Add("data_coding", data_coding);
      return call_api_method(map);
      // return (String)"details"
    }

    public Hashtable message_status(String message_id)
    {
      /* Result: {"status":0,"message":"OK","details":{"status":1,"cost":0.00000,"parts":1}}
       *
       * Hashtable [
       *   "status",
       *   "message",
       *   Hashtable "details" [
       *     "status",
       *     "cost",
       *     "parts"
       *   ]
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "message_status");
      map.Add("message_id", message_id);
      return call_api_method(map);
      // return (Hashtable)"details"
    }

    public Hashtable ping()
    {
      Hashtable map = new Hashtable();
      map.Add("action", "ping");
      return call_api_method(map);
    }

    public Hashtable user_get_balance()
    {
      /* Result: {"status":0,"message":"OK","details":3338.50000}
       *
       * Hashtable [
       *   "status",
       *   "message",
       *   "details"
       * ]
       */

      Hashtable map = new Hashtable();
      map.Add("action", "user_get_balance");
      return call_api_method(map);
      // return (double)"details"
    }
  }
}

