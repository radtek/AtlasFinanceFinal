/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *    HTTP Post helper class    
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-09-03 - Initial revision
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Web;


namespace Atlas.Common.Utils
{
  /// <summary>
  /// Simple HTTP helper to post data  to a provided URL
  /// </summary>
  public class Http
  {
    #region Enums

    /// <summary>
    /// determines what type of post to perform.
    /// </summary>
    public enum PostTypeEnum
    {
      /// <summary>
      /// Does a get against the source.
      /// </summary>
      Get,

      /// <summary>
      /// Does a post against the source.
      /// </summary>
      Post
    }

    #endregion

    #region Private Members

    private string _Url = string.Empty;
    private NameValueCollection _values = new NameValueCollection();
    private PostTypeEnum _type = PostTypeEnum.Get;

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor.
    /// </summary>
    public Http()
    {
    }

    /// <summary>
    /// Constructor that accepts a url as a parameter
    /// </summary>
    /// <param name="url">The url where the post will be submitted to.</param>
    public Http(string url)
      : this()
    {
      _Url = url;
    }

    /// <summary>
    /// Constructor allowing the setting of the url and items to post.
    /// </summary>
    /// <param name="url">the url for the post.</param>
    /// <param name="values">The values for the post.</param>
    public Http(string url, NameValueCollection values, int timeOut)
      : this(url)
    {
      _values = values;
      TimeOut = timeOut;
    }

    #endregion


    #region Public Properties

    /// <summary>
    /// Gets or sets the url to submit the post to.
    /// </summary>
    public string Url
    {
      get { return _Url; }
      set { _Url = value; }
    }

    /// <summary>
    /// Gets or sets the name value collection of items to post.
    /// </summary>
    public NameValueCollection PostItems
    {
      get { return _values; }
      set { _values = value; }
    }

    /// <summary>
    /// Gets or sets the type of action to perform against the url.
    /// </summary>
    public PostTypeEnum Type
    {
      get { return _type; }
      set { _type = value; }
    }


    /// <summary>
    /// Sets the timeout of the web request.
    /// </summary>
    public int TimeOut { get; set; }

    #endregion


    #region Public Methods

    /// <summary>
    /// Posts the supplied data to specified url.
    /// </summary>
    /// <returns>a string containing the result of the post.</returns>
    public string Post()
    {
      var parameters = new StringBuilder();
      for (var i = 0; i < _values.Count; i++)
      {
        EncodeAndAddItem(ref parameters, _values.GetKey(i), _values[i]);
      }
      return PostData(_Url, parameters.ToString(), (int)TimeOut);      
    }


    /// <summary>
    /// Posts the supplied data to specified url.
    /// </summary>
    /// <param name="url">The url to post to.</param>
    /// <returns>a string containing the result of the post.</returns>
    public string Post(string url)
    {
      _Url = url;
      return Post();
    }


    /// <summary>
    /// Posts the supplied data to specified url.
    /// </summary>
    /// <param name="url">The url to post to.</param>
    /// <param name="values">The values to post.</param>
    /// <returns>a string containing the result of the post.</returns>
    public string Post(string url, NameValueCollection values)
    {
      _values = values;
      return Post(url);
    }

    #endregion


    #region Private Methods

    /// <summary>
    /// Posts data to a specified url. Note that this assumes that you have already url encoded the post data.
    /// </summary>
    /// <param name="postData">The data to post.</param>
    /// <param name="url">the url to post to.</param>
    /// <returns>Returns the result of the post.</returns>
    private string PostData(string url, string postData, int timeOut)
    {
      HttpWebRequest request = null;
      if (_type == PostTypeEnum.Post)
      {
        var uri = new Uri(url);
        request = (HttpWebRequest)WebRequest.Create(uri);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = postData.Length;
        request.Timeout = timeOut;

        using (var writeStream = request.GetRequestStream())
        {
          var encoding = new UTF8Encoding();
          var bytes = encoding.GetBytes(postData);
          writeStream.Write(bytes, 0, bytes.Length);
        }
      }
      else
      {
        var uri = new Uri(string.Format("{0}?{1}", url, postData));
        request = (HttpWebRequest)WebRequest.Create(uri);
        request.Method = "GET";
      }
      var result = string.Empty;
      using (var response = (HttpWebResponse)request.GetResponse())
      {
        using (var responseStream = response.GetResponseStream())
        {
          using (var readStream = new StreamReader(responseStream, Encoding.UTF8))
          {
            result = readStream.ReadToEnd();
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Encodes an item and adds it to the string.
    /// </summary>
    /// <param name="baseRequest">The previously encoded data.</param>
    /// <param name="dataItem">The data to encode.</param>
    /// <returns>A string containing the old data and the previously encoded data.</returns>
    private static void EncodeAndAddItem(ref StringBuilder baseRequest, string key, string dataItem)
    {
      if (baseRequest == null)
      {
        baseRequest = new StringBuilder();
      }
      if (baseRequest.Length != 0)
      {
        baseRequest.Append("&");
      }
      baseRequest.Append(key);
      baseRequest.Append("=");
      baseRequest.Append(HttpUtility.UrlEncode(dataItem));
    }

    #endregion

  }
}