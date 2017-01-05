﻿#region Dapplo 2016 - GNU Lesser General Public License

// Dapplo - building blocks for .NET applications
// Copyright (C) 2017 Dapplo
// 
// For more information see: http://dapplo.net/
// Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
// This file is part of Dapplo.Confluence
// 
// Dapplo.Confluence is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Dapplo.Confluence is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have a copy of the GNU Lesser General Public License
// along with Dapplo.Confluence. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#endregion

namespace Dapplo.Confluence.Query
{
	/// <summary>
	///     Factory method for CQL where clauses
	/// </summary>
	public static class Where
	{
		#region User based clauses
		/// <summary>
		///     Create a clause for the creator
		/// </summary>
		public static IUserClause Creator => new UserClause(Fields.Creator);

		/// <summary>
		///     Create a clause for the contributor
		/// </summary>
		public static IUserClause Contributor => new UserClause(Fields.Contributor);

		/// <summary>
		///     Create a clause for the mention
		/// </summary>
		public static IUserClause Mention => new UserClause(Fields.Mention);

		/// <summary>
		///     Create a clause for the watcher
		/// </summary>
		public static IUserClause Watcher => new UserClause(Fields.Watcher);

		/// <summary>
		///     Create a clause for the favourite
		/// </summary>
		public static IUserClause Favourite => new UserClause(Fields.Favourite);

		#endregion

		/// <summary>
		///     Create a clause for the created field
		/// </summary>
		public static IDatetimeClause Created => new DatetimeClause(Fields.Created);

		/// <summary>
		///     Create a clause for the lastmodified field
		/// </summary>
		public static IDatetimeClause LastModified => new DatetimeClause(Fields.LastModified);

		/// <summary>
		///     Create a clause for the type field
		/// </summary>
		public static ITypeClause Type => new TypeClause();

		#region BooleanLogic

		public static string And(IFinalClause clause1, string clause2)
		{
			return $"({clause1} and {clause2})";
		}

		public static string Or(IFinalClause clause1, string clause2)
		{
			return $"({clause1} or {clause2})";
		}

		public static string And(IFinalClause clause1, IFinalClause clause2)
		{
			return $"({clause1} and {clause2})";
		}

		public static string Or(IFinalClause clause1, IFinalClause clause2)
		{
			return $"({clause1} or {clause2})";
		}

		public static string And(string clause1, IFinalClause clause2)
		{
			return $"({clause1} and {clause2})";
		}

		public static string Or(string clause1, IFinalClause clause2)
		{
			return $"({clause1} or {clause2})";
		}

		public static string And(string clause1, string clause2)
		{
			return $"({clause1} and {clause2})";
		}

		public static string Or(string clause1, string clause2)
		{
			return $"({clause1} or {clause2})";
		}

		#endregion
	}
}