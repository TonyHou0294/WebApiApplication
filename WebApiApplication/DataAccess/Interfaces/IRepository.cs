using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebApiApplication.DataAccess.Interfaces
{
    public interface IRepository<T>
    {
		/// <summary>
		/// 新增資料
		/// </summary>
		/// <returns></returns>
		void Insert(T entity);

		/// <summary>
		/// 新增多筆資料
		/// </summary>
		/// <returns></returns>
		void InsertRange(IEnumerable<T> entities);

		/// <summary>
		/// 修改資料
		/// </summary>
		/// <returns></returns>
		void Update(T entity);

		/// <summary>
		/// 刪除資料
		/// </summary>
		/// <returns></returns>
		void Delete(T entity);

		/// <summary>
		/// 刪除多筆資料
		/// </summary>
		/// <returns></returns>
		void DeleteRange(IEnumerable<T> entities);

		/// <summary>
		/// 指定條件查詢資料
		/// </summary>
		/// <returns>IQueryable查詢結果</returns>
		IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate);

		/// <summary>
		/// 查詢全部資料
		/// </summary>
		/// <returns>IQueryable查詢結果</returns>
		IQueryable<T> GetAll();

		/// <summary>
		/// 排除已刪除（假性刪除）的資料
		/// </summary>
		/// <returns>IQueryable查詢結果</returns>
		IQueryable<T> GetAvailable();

		/// <summary>
		/// 指定條件查詢資料
		/// </summary>
		/// <returns>符合查詢條件第一筆資料結果</returns>
		T Get(Expression<Func<T, bool>> predicate);

		/// <summary>
		/// 主鍵查詢資料
		/// </summary>
		/// <returns>符合查詢條件第一筆資料結果</returns>
		T Find(params object[] keys);

		/// <summary>
		/// 指定條件查詢資料
		/// </summary>
		/// <returns>符合查詢條件唯一的一筆資料結果</returns>
		T Find(Expression<Func<T, bool>> predicate);

		/// <summary>
		/// 主鍵刪除資料
		/// </summary>
		/// <returns></returns>
		void DeleteByKey(params object[] keys);

		/// <summary>
		/// 主鍵刪除多筆資料
		/// </summary>
		/// <returns></returns>
		void DeleteItemsByKeys(object[] keys);


		/// <summary>
		/// 儲存資料庫
		/// </summary>
		/// <returns>異動筆數</returns>
		int SaveChanges();
	}
}
