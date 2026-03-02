using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Common;
using Ventas.Infrastructure.Data;
using Ventas.Infrastructure.Exceptions;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class BaseRepository<T>(AppDbContext context) : IBaseRepository<T> where T : BaseModel
    {
        private readonly AppDbContext _context = context;
        private readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task<T> CreateAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                return entity;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error al insertar en la base de datos.", ex);
            }
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
                return true;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error al eliminar en la base de datos.", ex);
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _dbSet.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error al consultar en la base de datos.", ex);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, IEnumerable<Func<IQueryable<T>, IQueryable<T>>>? includes = null, bool disableTracking = true)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>();

                if(includes != null)
                {
                    foreach(var include in includes)
                    {
                        query = include(query);
                    }
                }

                if(predicate != null)
                {
                    query = query.Where(predicate);
                }

                if(disableTracking)
                {
                    query = query.AsNoTracking();
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error al consultar en la base de datos.", ex);
            }
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error al obtener por id en la base de datos.", ex);
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                var trackedEntity = await _dbSet.FindAsync(entity.Id);

                if(trackedEntity != null)
                {
                    _dbSet.Entry(trackedEntity).CurrentValues.SetValues(entity);
                    _dbSet.Entry(trackedEntity).State = EntityState.Modified;
                }
                else
                {
                    _dbSet.Entry(entity).State = EntityState.Added;
                }

                return entity;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error al actualizar en la base de datos.", ex);
            }
        }
    }
}
