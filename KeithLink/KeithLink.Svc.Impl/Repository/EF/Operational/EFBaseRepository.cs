using KeithLink.Svc.Core.Models.EF;
using Microsoft.Practices.Unity.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.EF.Operational
{
	public abstract class EFBaseRepository<T> where T : BaseEFModel
	{
		public IUnitOfWork UnitOfWork { get; private set; }

		protected DbSet<T> Entities
		{
			get { return this.UnitOfWork.Context.Set<T>(); }
		}

		protected EFBaseRepository(IUnitOfWork unitOfWork)
		{
			this.UnitOfWork = unitOfWork;
		}

		public virtual void Create(T entity)
		{
			this.Entities.Add(entity);
		}

		public virtual void CreateOrUpdate(T domainObject)
		{
			if (domainObject == null)
			{
				var message = "Parameter of type {0} cannot be null.";
				message = string.Format(message, typeof(T).ToString());

				throw new ArgumentNullException("domainObject", message);
			}

			var context = UnitOfWork.Context;
			var transient = ReadTransient(domainObject);
			if (transient != null)
			{
				context.Entry(transient).CurrentValues.SetValues(domainObject);
				UnitOfWork.Context.Entry(transient).State = EntityState.Modified;
			}
			else
			{
				this.Create(domainObject);
			}
		}

		public virtual IEnumerable<T> ReadAll()
		{
			return this.Entities;
		}

		public virtual IEnumerable<T> Read(Expression<Func<T, bool>> where)
		{
			return this.Entities.Where(where);
		}

		public virtual IEnumerable<T> Read<TProperty>(Expression<Func<T, TProperty>> include) where TProperty : class
		{
			return this.Entities.Include(include);
		}

		public virtual IEnumerable<T> Read<TProperty>(Expression<Func<T, bool>> where, Expression<Func<T, TProperty>> include) where TProperty : class
		{
			return this.Entities.Where(where).Include(include);
		}

		public virtual T ReadById(long id)
		{
			T output;
			
			var transient = ReadTransient(id);
			if (transient != null)
			{
				output = transient;
			}
			else
			{
				output = this.Entities.SingleOrDefault(x => x.Id == id);
			}

			return output;
		}

		public virtual T ReadTransient(T domainObject)
		{
			return ReadTransient(domainObject.Id);
		}

		public virtual T ReadTransient(long id)
		{
			if (id == 0)
				return null;

			T output = null;

			var transient = Entities.Find(id);
			if (transient != null)
			{
				output = transient;
			}

			return output;
		}

		public virtual void Update(T domainObject)
		{
			var context = UnitOfWork.Context;
			var transient = ReadTransient(domainObject);

			if (transient == null)
			{
				this.Entities.Attach(domainObject);
			}
			else
			{
				context.Entry(transient).CurrentValues.SetValues(domainObject);
				domainObject = transient;
			}

			context.Entry(domainObject).State = EntityState.Modified;
		}

		public virtual void Delete(Expression<Func<T, bool>> where)
		{
			IEnumerable<T> objects = this.Entities.Where<T>(where).AsEnumerable();
			foreach (T obj in objects)
				this.Entities.Remove(obj);
		}

		public virtual void Delete(T domainObject)
		{
			this.Entities.Remove(domainObject);
		}
	}
}
