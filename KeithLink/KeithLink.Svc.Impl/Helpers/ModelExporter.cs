using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Helpers
{
	public class ModelExporter<TModel> where TModel : class, IExportableModel
	{
		private IList<TModel> Model { get; set; }

		//private List<ModelExportEnumMap> _enumMaps = null;
		//public IReadOnlyList<ModelExportEnumMap> EnumMaps { get { return this._enumMaps.AsReadOnly(); } }
		private List<ExportModelConfiguration> exportConfig = null;
						
		public ModelExporter(IList<TModel> model, List<ExportModelConfiguration> exportConfig)
		{
			this.Model = model;
			this.exportConfig = exportConfig;
		}

		public ModelExporter(IList<TModel> model)
		{
			this.Model = model;
			this.exportConfig = model.First().DefaultExportConfiguration();
		}

		//public void AddEnumMap<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, Type enumerationType)
		//{
		//	var property = this.GetMemberInfo(propertyExpression);
		//	if (property is PropertyInfo)
		//		this._enumMaps.Add(new ModelExportEnumMap(property.Name, enumerationType));
		//	else
		//		throw new ArgumentException("Expression is not a property access", "propertyExpression");
		//}

		private MemberInfo GetMemberInfo<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
		{
			var member = propertyExpression.Body as MemberExpression;
			if (member != null)
				return member.Member;

			throw new ArgumentException("Expression is not a member access", "expression");
		}

		public MemoryStream Export(string exportType)
		{
			StringBuilder sb = new StringBuilder();

			if(exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase) || exportType.Equals("tab", StringComparison.CurrentCultureIgnoreCase))
				this.WriteHeaderRecord(sb, exportType);

			if (this.Model != null && this.Model.Count > 0) // is there any data to render
			{
				foreach (var item in this.Model)
				{
					if (item != null)
						this.WriteItemRecord(sb, item, exportType);
				}

			}
			var ms = new MemoryStream();
			var sw = new StreamWriter(ms);

			sw.Write(sb.ToString());
			sw.Flush();

			ms.Seek(0, SeekOrigin.Begin);
			return ms;
		}

		private void WriteItemRecord(StringBuilder sb, TModel item, string exportType)
		{
			List<string> itemRecord = new List<string>();

			var properties = item.GetType().GetProperties();

			foreach (var config in exportConfig.OrderBy(e => e.Order))
			{
				var property = properties.Where(p => p.Name.Equals(config.Field, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
				if (property != null)
				{
					itemRecord.Add(string.Format("\"{0}\"", this.GetFieldValue(item, property).Trim()));
				}
			}
			
			if (itemRecord.Count > 0)
				sb.AppendLine(string.Join(exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase ) ? "," : "\t", itemRecord));
		}

		private string GetFieldValue(object item, PropertyInfo property)
		{
			var value = property.GetValue(item);
			if (value == null)
				return string.Empty;

			if (value.GetType().IsEnum)
				return this.GetAttributeFieldValue(value.GetType(), value.ToString());

			return value.ToString();
		}

		private string GetAttributeFieldValue(Type enumerationType, string fieldName)
		{
			FieldInfo field = enumerationType.GetField(fieldName);
			if (field != null)
			{
				DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
				if (attr != null)
					return attr.Description;
			}
			return fieldName;
		}

		private void WriteHeaderRecord(StringBuilder sb, string exportType)
		{
			var headerRecord = new List<string>();

			var properties = typeof(TModel).GetProperties();

			foreach (var config in exportConfig.OrderBy(e => e.Order))
			{
				var property = properties.Where(p => p.Name.Equals(config.Field, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
				if (property != null)
				{
					var description = property.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
					string value = string.Empty;
					if (description != null)
						value = description.Description;
					else
						value = property.Name;

					headerRecord.Add(string.Format("\"{0}\"", value.Trim()));
				}
			}

			sb.AppendLine(string.Join(exportType.Equals("csv", StringComparison.CurrentCultureIgnoreCase) ? "," : "\t", headerRecord));
		}

	}
}
