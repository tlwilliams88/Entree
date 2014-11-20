using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Paging
{
	[DataContract]
	public class FilterInfo
	{
		public ConditionalLogic Logic { get { return ParseLogic(this.Condition); } }

		[DataMember(Name = "condition")]
		public string Condition { get; set; }

		/// <summary>
		/// The name of the field or property of the entity
		/// </summary>
		[DataMember(Name = "field")]
		public string Field { get; set; }

		/// <summary>
		/// The actual expression - used if the field name is not the actual value that should be evaluated. i.e., if the property / field 
		/// is CustomerName, but the actual value that should be evaluated is a sub property of CustomerName (CustomerName.Value)
		/// </summary>
		public string FieldExpression { get; set; }

		[DataMember(Name = "type")]
		public string FilterType { get; set; }

		public Operator Operator { get { return ParseOperator(this.FilterType); } }

		[DataMember(Name = "value")]
		public string Value { get; set; }

		[DataMember(Name = "filter")]
		public List<FilterInfo> Filters { get; set; }


		public static ConditionalLogic ParseLogic(string logic)
		{
			if (string.IsNullOrWhiteSpace(logic))
				return ConditionalLogic.And;

			switch (logic.ToLower())
			{
				case "&":
				case "&&":
				case "and":
					return ConditionalLogic.And;
				case "|":
				case "||":
				case "or":
					return ConditionalLogic.Or;
				default:
					return ConditionalLogic.And;
			}
		}

		public static Operator ParseOperator(string theOperator)
		{
			if (string.IsNullOrWhiteSpace(theOperator))
				return Operator.Contains;

			switch (theOperator.ToLower())
			{

				//equal ==
				case "eq":
				case "==":
				case "isequalto":
				case "equals":
				case "equalto":
				case "equal":
					return Operator.Equals;

				//not equal !=
				case "neq":
				case "!=":
				case "isnotequalto":
				case "notequals":
				case "notequalto":
				case "notequal":
				case "ne":
					return Operator.NotEquals;

				// Greater
				case "gt":
				case ">":
				case "isgreaterthan":
				case "greaterthan":
				case "greater":
					return Operator.GreaterThan;

				// Greater or equal
				case "gte":
				case ">=":
				case "isgreaterthanorequalto":
				case "greaterthanequal":
				case "ge":
					return Operator.GreaterThanOrEquals;

				// Less
				case "lt":
				case "<":
				case "islessthan":
				case "lessthan":
				case "less":
					return Operator.LessThan;

				// Less or equal
				case "lte":
				case "<=":
				case "islessthanorequalto":
				case "lessthanequal":
				case "le":
					return Operator.LessThanOrEquals;

				case "startswith":
					return Operator.StartsWith;

				case "endswith":
					return Operator.EndsWith;

				case "contains":
					return Operator.Contains;

				case "doesnotcontain":
					return Operator.NotContains;

				default:
					return Operator.Contains;
			}

		}
	}

	public enum Operator
	{
		Equals,
		NotEquals,
		GreaterThan,
		GreaterThanOrEquals,
		LessThan,
		LessThanOrEquals,
		StartsWith,
		EndsWith,
		Contains,
		NotContains,
		Undefined,
	}
	
	public enum ConditionalLogic
	{
		And,
		Or,
		Undefined
	}

	
}
