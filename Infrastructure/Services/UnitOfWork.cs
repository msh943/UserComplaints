using Core.Domain;
using Core.IService;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T :class
    {

        private readonly AppDbContext _context;
        internal DbSet<T> dbSet;
        public UnitOfWork(AppDbContext context)
        {
           
            _context = context;
            this.dbSet = _context.Set<T>();
        }
        public async Task<T> Create(T complaints)
        {
            
            await dbSet.AddAsync(complaints);
            await Save();

            return complaints;
        }

        public async Task<T> Get(Expression<Func<T, bool>>? filter = null, bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);

            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);

            }
            return await query.ToListAsync();
        }

        public async Task Remove(T complaint)
        {
            dbSet.Remove(complaint);
            await Save();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }


    }
}
