using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorcycleEmissionDAL
{
	public class GenericRepository<T> where T : class
	{
		private readonly MotorcycleEmissionDbContext _context;

		public GenericRepository()
		{
			_context = new();
		}
		public void Add(T entity)
		{
			_context.Set<T>().Add(entity);
			_context.SaveChanges();
		}

		// Delete an entity
		public void Delete(T entity)
		{
			_context.Set<T>().Remove(entity);
			_context.SaveChanges();
		}

		public void DeleteById(int id)
		{
			var entity = GetById(id);
			if (entity != null)
			{
				Delete(entity);
			}
		}

		// Update an entity
		public void Update(T entity)
		{
			_context.Set<T>().Update(entity);
			_context.SaveChanges();
		}

		// Get all entities
		public List<T> GetAll()
		{
			return _context.Set<T>().ToList();
		}

		// Get a single entity by ID
		public T GetById(int id)
		{
			return _context.Set<T>().Find(id);
		}
	}
}
