using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Web.Service.Hashing
{
	public static class Hash
	{
		public static string Generate(Guid correlationId, long clientId)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0}{1}",correlationId,clientId));
			byte[] buffer = new SHA256Managed().ComputeHash(bytes);
			string str = string.Empty;
			foreach (byte num in buffer)
			{
				str = str + string.Format("{0:x2}", num);
			}
			return str;
		}
	}
}
