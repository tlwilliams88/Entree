using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using KeithLink.Svc.Core.Models.Paging;

namespace KeithLink.Svc.Core.Extensions
{
	public static class Paging
	{
		public const string REGEX_TIME = "(([0-1][0-9])|([2][0-3])):([0-5][0-9]):([0-5][0-9])";

		public static PagedResults<TEntity> GetPage<TEntity>(this IQueryable<TEntity> queryable, PagingModel page, string defaultSortPropertyName = null, string[] multipleValuePropertyNames = null)
		{
			return new PagedResults<TEntity>()
			{
				Results = queryable
					.Filter(page.Filter, multipleValuePropertyNames)
					.Sort(page.Sort, defaultSortPropertyName ?? typeof(TEntity).GetProperties().First().Name)
					.Skip(page.From.HasValue ? page.From.Value : 0)
					.Take(page.Size.HasValue ? page.Size.Value : int.MaxValue)
					.ToList(),
				TotalResults = queryable
					.Filter(page.Filter, multipleValuePropertyNames)
					.Count(),
			};
		}

		#region FILTER

		public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> queryable, FilterInfo filter, string[] multipleValuePropertyNames)
		{
			if (filter == null)
				return queryable;

			return queryable.Where(BuildFilterExpresion<TEntity>(filter, filter.Logic, multipleValuePropertyNames));
		}

		private static string BuildFilterExpresion<TEntity>(FilterInfo activeFilter, ConditionalLogic parentLogic, string[] multipleValuePropertyNames)
		{
			// build inner expression string

			string innerExpression = null;

			if (activeFilter.Filters != null)
			{
				foreach (var innerFilter in activeFilter.Filters)
				{
					var expression = BuildFilterExpresion<TEntity>(innerFilter, activeFilter.Logic, multipleValuePropertyNames);

					if (innerExpression == null)
						innerExpression = expression;
					else
						innerExpression = string.Format("{0} {1} {2}",
							innerExpression,
							activeFilter.Logic == ConditionalLogic.And ? "&&" : "||",
							expression);
				}
			}

			// if the active expression is the root expression, then the field, operator and value will be undefined - so, simply return the inner expressions
			// inner expression will not be empty or null in this case

			if (string.IsNullOrWhiteSpace(activeFilter.Field))
				return innerExpression;

			// build active expression

			string activeExpression = null;
			if (multipleValuePropertyNames != null && multipleValuePropertyNames.Contains(activeFilter.Field)) // multiple values
			{
				activeExpression = BuildExpression<TEntity>(activeFilter.Field, activeFilter.FieldExpression, activeFilter.Operator,
					activeFilter.Value, multipleValuePropertyNames);
			}
			else // single value
			{
				activeExpression = BuildExpression<TEntity>(activeFilter.Field, activeFilter.FieldExpression, activeFilter.Operator,
					activeFilter.Value);
			}

			// return combined expression

			if (string.IsNullOrWhiteSpace(innerExpression))
				return activeExpression; // return active expression if inner expression is null or empty
			else
				return string.Format("({0}) {1} ({2})",
					activeExpression,
					parentLogic == ConditionalLogic.And ? "&&" : "||",
					innerExpression);
		}

		private static string BuildExpression<TEntity>(string fieldName, string fieldExpression, Operator op, string value, string[] multipleValuePropertyNames)
		{
			var multiValueExpressions = new List<string>();
			foreach (var item in value.Split(',')) // splits on comma because this is how data is passed up from kendo grid regardless of commas in values
				multiValueExpressions.Add(BuildExpression<TEntity>(fieldName, fieldExpression, op, item));

			switch (op)
			{
				case Operator.Equals:
				case Operator.Contains:
				case Operator.GreaterThan:
				case Operator.GreaterThanOrEquals:
				case Operator.StartsWith:
					return string.Join(" || ", multiValueExpressions);

				case Operator.LessThanOrEquals:
				case Operator.EndsWith:
				case Operator.LessThan:
				case Operator.NotContains:
				case Operator.NotEquals:
				case Operator.Undefined:
				default:
					return string.Join(" && ", multiValueExpressions);
			}
		}

		private static string BuildExpression<TEntity>(string fieldName, string fieldExpression, Operator op, string value)
		{
			var propertyExpression = fieldExpression ?? fieldName;
			var fieldType = GetFieldType<TEntity>(propertyExpression);

			//Type fieldType = typeof(TEntity).GetProperty(fieldName, 
			//    System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
			//    .PropertyType;

			// get the formatted value

			string formattedValue = value;

			if (fieldType == typeof(char))
				formattedValue = string.Format("'{0}'", value);
			else if (fieldType == typeof(string))
				formattedValue = string.Format("\"{0}\"", value);
			else if (fieldType == typeof(DateTime))
				formattedValue = GetDateExpression(value);
			else if (fieldType == typeof(TimeSpan))
				formattedValue = GetTimeSpanExpression(value);
			else if (fieldType.IsEnum)
				formattedValue = string.Format("\"{0}\"", GetEnumValueExpression(fieldType, value));

			if (fieldType != typeof(string) && op == Operator.Contains)
				op = Operator.Equals;


			// build expression

			switch (op)
			{
				case Operator.Equals:
					return string.Format("{0} == {1}", propertyExpression, formattedValue);
				case Operator.NotEquals:
					return string.Format("{0} != {1}", propertyExpression, formattedValue);
				case Operator.GreaterThan:
					return string.Format("{0} > {1}", propertyExpression, formattedValue);
				case Operator.GreaterThanOrEquals:
					return string.Format("{0} >= {1}", propertyExpression, formattedValue);
				case Operator.LessThan:
					return string.Format("{0} < {1}", propertyExpression, formattedValue);
				case Operator.LessThanOrEquals:
					return string.Format("{0} <= {1}", propertyExpression, formattedValue);
				case Operator.StartsWith:
					CheckForStringTypeOperator(propertyExpression, fieldType, Operator.StartsWith);
					return string.Format("{0}.StartsWith({1})", propertyExpression, formattedValue);
				case Operator.EndsWith:
					CheckForStringTypeOperator(propertyExpression, fieldType, Operator.EndsWith);
					return string.Format("{0}.EndsWith({1})", propertyExpression, formattedValue);
				case Operator.Contains:
					CheckForStringTypeOperator(propertyExpression, fieldType, Operator.Contains);
					return string.Format("({0} != null AND {0}.ToUpper().Contains({1}))", propertyExpression, formattedValue.ToUpper());
				case Operator.NotContains:
					CheckForStringTypeOperator(propertyExpression, fieldType, Operator.NotContains);
					return string.Format("!{0}.ToUpper().Contains({1})", propertyExpression, formattedValue.ToUpper());
				default:
					throw new Exception(string.Format("Operator '{0}' not defined for switch", op));
			}

		}

		private static Type GetFieldType<TEntity>(string propertyExpression)
		{
			Stack<string> fieldPathStack =
				new Stack<string>(propertyExpression.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Reverse());

			var currentType = typeof(TEntity);

			do
			{
				currentType = currentType.GetProperty(fieldPathStack.Pop(),
					System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
					.PropertyType;
			} while (fieldPathStack.Count > 0);

			return currentType;
		}

		private static string GetTimeSpanExpression(string value)
		{
			var match = new Regex(REGEX_TIME).Match(value).Value;
			var timeSpan = TimeSpan.Parse(match);

			return string.Format("TimeSpan({0}, {1}, {2})",
				timeSpan.Hours,
				timeSpan.Minutes,
				timeSpan.Seconds);
		}

		private static string GetDateExpression(string value)
		{
			var dt = DateTime.Parse(value);
			return string.Format("DateTime({0}, {1}, {2})",
				dt.Year,
				dt.Month,
				dt.Day);
		}

		private static string GetEnumValueExpression(Type fieldType, string value)
		{
			// try to get the name by value first

			if (Char.IsNumber(value.Trim()[0]))
				return Enum.GetName(fieldType, Convert.ChangeType(value, Enum.GetUnderlyingType(fieldType)));
			else
				return Enum.Parse(fieldType, value.Trim()).ToString();
		}

		private static void CheckForStringTypeOperator(string fieldName, Type fieldType, Operator op)
		{
			switch (op)
			{
				case Operator.Equals:
				case Operator.NotEquals:
				case Operator.GreaterThan:
				case Operator.GreaterThanOrEquals:
				case Operator.LessThan:
				case Operator.LessThanOrEquals:
					return;
				case Operator.StartsWith:
				case Operator.EndsWith:
				case Operator.Contains:
				case Operator.NotContains:
					if (fieldType != typeof(string))
						throw new Exception(string.Format("The operator '{0}' requires a field type of 'string' for field '{1}'", op, fieldName));
					return;
				default:
					throw new Exception(string.Format("Operator '{0}' not defined for switch", op));
			}
		}

		#endregion

		#region SORT

		public static IQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> queryable, List<SortInfo> sort, string defaultOrderByPropertyName = null)
		{
			if (sort == null || sort.Count == 0) // sort collection is null or empty so sorty by a default
			{
				return queryable.OrderBy(string.Format("{0} {1}", defaultOrderByPropertyName, "asc"));
			}
			else
			{
				var sortList = new List<string>();
				sortList.AddRange(sort.Select(i => string.Format("{0} {1}", i.Field, i.SortOrder.ToString())));
				return queryable.OrderBy(string.Join(",", sortList));
			}
		}

		#endregion
	}
}
