 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avarez
{
	public static class Config
	{
		#region Public Properties
		public static string baseUploadDir
		{
			get
			{
				return ("~/HelpPhoto");
			}
		}

		public static string baseUploadDirPhysical
		{
			get
			{
				return (HttpContext.Current.Server.MapPath(Config.baseUploadDir));
			}
		}

		public static string[] allowedUploadExtensions
		{
			get
			{
				return (new string[] { "png", "gif", "jpg", "jpeg", "zip", "doc", "docx" });
			}
		}
		#endregion
	}
}