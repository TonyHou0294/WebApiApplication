using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using WebApiApplication.DataAccess.Interfaces;
using WebApiApplication.Models;

namespace WebApiApplication.DataAccess
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
		private readonly WebApiDBContext _db;
		protected DbSet<TEntity> Set;

		public EfRepository(WebApiDBContext db)
		{
			_db = db;
			Set = db.Set<TEntity>();
		}

		public void Insert(TEntity entity)
		{
			Set.Add(entity);
		}

		public void InsertRange(IEnumerable<TEntity> entities)
		{
			Set.AddRange(entities);
		}

		public void Update(TEntity entity)
		{
			_db.Entry(entity).State = EntityState.Modified;
		}

		public void Delete(TEntity entity)
		{
			Set.Remove(entity);
		}

		public void DeleteRange(IEnumerable<TEntity> entities)
		{
			Set.RemoveRange(entities);
		}

		public IQueryable<TEntity> SearchFor(Expression<Func<TEntity, bool>> predicate)
		{
			return Set.Where(predicate);
		}

		public virtual IQueryable<TEntity> GetAll()
		{
			return Set;
		}

		public virtual IQueryable<TEntity> GetAvailable()
		{
			var queryable = GetAll();

			if (typeof(IDeleteState).IsAssignableFrom(typeof(TEntity)))
			{
				var alternate = (IQueryable<IDeleteState>)queryable;

				queryable = alternate.Where(x => x.IsDeleted == false).Cast<TEntity>();
			}

			return queryable;
		}

		public TEntity Get(Expression<Func<TEntity, bool>> predicate)
		{
			return Set.FirstOrDefault(predicate);
		}

		public TEntity Find(params object[] keys)
		{
			return Set.Find(keys);
		}

		public TEntity Find(Expression<Func<TEntity, bool>> predicate)
		{
			return Set.SingleOrDefault(predicate);
		}

		public void DeleteByKey(params object[] keys)
		{
			var item = Set.Find(keys);
			if (item == null) return;
			if (typeof(IDeleteState).IsAssignableFrom(typeof(TEntity)))
			{
				var alternate = (IDeleteState)item;
				alternate.IsDeleted = true;
				Update(item);
			}
			else
			{
				Delete(item);
			}
		}

		public virtual void DeleteItemsByKeys(object[] keys)
		{
			foreach (var key in keys)
				DeleteByKey(key);
		}

		public int SaveChanges()
		{
			return _db.SaveChanges();
		}
	}
}