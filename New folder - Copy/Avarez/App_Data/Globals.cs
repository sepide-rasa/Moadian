using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Avarez
{
	public static class Globals
	{
		#region Private Members
		private static JavaScriptSerializer _serializer=new JavaScriptSerializer();
		private static string applicationPath=System.Web.Hosting.HostingEnvironment.MapPath("~/");
		#endregion

		#region Public Properties
		public static JavaScriptSerializer serializer
		{
			get
			{
				return (Globals._serializer);
			}
		}
		#endregion

		#region Public Methods
		public static string resolveVirtual (string physicalPath)
		{
			string url=physicalPath.Substring(Globals.applicationPath.Length).Replace('\\', '/').Insert(0, "~/");
			return (url);
		}

		public static string resolveUrl (string originalUrl)
		{
			if (originalUrl == null)
				return null;

			// *** Absolute path - just return
			if (originalUrl.IndexOf("://") != -1)
				return originalUrl;

			// *** Fix up image path for ~ root app dir directory
			if (originalUrl.StartsWith("~"))
			{
				string newUrl = "";
				if (HttpContext.Current != null)
					newUrl = (HttpContext.Current.Request.ApplicationPath +
						  originalUrl.Substring(1)).Replace("//", "/");
				else
					// *** Not context: assume current directory is the base directory
					throw new ArgumentException("Invalid URL: Relative URL not allowed.");

				// *** Just to be sure fix up any double slashes
				return newUrl;
			}

			return originalUrl;
		}
		#endregion 
	}
}