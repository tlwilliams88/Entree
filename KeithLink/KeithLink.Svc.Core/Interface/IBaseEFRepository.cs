﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface
{
	public interface IBaseEFREpository<T> where T : class
	{
		// Using CRUD (Create, Read, Update, Delete) naming convention...
		void Create(T domainObject);
		void CreateOrUpdate(T domainObject);
		void Update(T domainObject);
		void Delete(T domainObject);
		T ReadById(long Id);
		IEnumerable<T> Read<TProperty>(Expression<Func<T, bool>> where, Expression<Func<T, TProperty>> include) where TProperty : class;
		IEnumerable<T> Read(Expression<Func<T, bool>> where);
	}
}