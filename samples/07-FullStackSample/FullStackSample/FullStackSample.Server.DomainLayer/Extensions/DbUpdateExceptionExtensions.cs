using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FullStackSample.Server.DomainLayer.Extensions
{
	public static class DbUpdateExceptionExtensions
	{
		private readonly static Regex Regex =
			new Regex("'ix_(\\w+)_(\\w+)'", RegexOptions.Compiled);


		public static bool IsUniqueIndexViolation(this DbUpdateException exception) =>
			exception.HResult == -2146233088
			&& exception.InnerException is SqlException
			&& exception.InnerException.HResult == -2146232060;

		public static KeyValuePair<string, string> GetViolationInfo(this DbUpdateException exception)
		{
			var match = Regex.Match(exception.InnerException.Message);
			if (!match.Success)
				throw new ArgumentException("Unique constraints must be in the format ix_TableName_ColumnName");

			return new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[2].Value);
		}

	}
}
