﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Common;
using HRM_AI.Repositories.Entities;
using HRM_AI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM_AI.Repositories.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly Guid? _currentUserId;
        protected DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context, IClaimService claimService)
        {
            _dbSet = context.Set<T>();
            _currentUserId = claimService.GetCurrentUserId;
        }

        public virtual async Task AddAsync(T entity)
        {
            entity.CreationDate = DateTime.UtcNow;
            entity.CreatedById = _currentUserId;
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(List<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreationDate = DateTime.UtcNow;
                entity.CreatedById = _currentUserId;
            }

            await _dbSet.AddRangeAsync(entities);
        }

        public virtual async Task<T?> GetAsync(Guid id, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;
            if (include != null) query = include(query);

            // TODO: Throw exception when result is not found
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual async Task<PaginationResult<List<T>>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? order = null, Func<IQueryable<T>, IQueryable<T>>? include = null,
            int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<T> query = _dbSet;

            // Filter
            if (filter != null) query = query.Where(filter);
            var totalCount = await query.CountAsync();

            // Order (sorting)
            if (order != null) query = order(query);

            // Include properties
            if (include != null) query = include(query);

            // Pagination
            // If pageIndex and pageSize are both null, return all items
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                var validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                var validPageSize = pageSize.Value > 0 ? pageSize.Value : Constant.DefaultMinPageSize;
                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            return new PaginationResult<List<T>>
            {
                TotalCount = totalCount,
                Data = await query.ToListAsync()
            };
        }

        public virtual void Update(T entity, bool? isOwnerRequired = false)
        {
            ValidateOwner(entity, isOwnerRequired);
            entity.ModificationDate = DateTime.UtcNow;
            entity.ModifiedById = _currentUserId;
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(List<T> entities, bool? isOwnerRequired = false)
        {
            foreach (var entity in entities)
            {
                ValidateOwner(entity, isOwnerRequired);
                entity.ModificationDate = DateTime.UtcNow;
                entity.ModifiedById = _currentUserId;
            }

            _dbSet.UpdateRange(entities);
        }

        public virtual void SoftRemove(T entity, bool? isOwnerRequired = false)
        {
            ValidateOwner(entity, isOwnerRequired);
            entity.IsDeleted = true;
            entity.DeletionDate = DateTime.UtcNow;
            entity.DeletedById = _currentUserId;
            _dbSet.Update(entity);
        }

        public virtual void SoftRemoveRange(List<T> entities, bool? isOwnerRequired = false)
        {
            foreach (var entity in entities)
            {
                ValidateOwner(entity, isOwnerRequired);
                entity.IsDeleted = true;
                entity.DeletionDate = DateTime.UtcNow;
                entity.DeletedById = _currentUserId;
            }

            _dbSet.UpdateRange(entities);
        }

        public virtual void Restore(T entity, bool? isOwnerRequired = false)
        {
            ValidateOwner(entity, isOwnerRequired);
            entity.IsDeleted = false;
            entity.DeletionDate = null;
            entity.DeletedById = null;
            entity.ModificationDate = DateTime.UtcNow;
            entity.ModifiedById = _currentUserId;
            _dbSet.Update(entity);
        }

        public virtual void RestoreRange(List<T> entities, bool? isOwnerRequired = false)
        {
            foreach (var entity in entities)
            {
                ValidateOwner(entity, isOwnerRequired);
                entity.IsDeleted = false;
                entity.DeletionDate = null;
                entity.DeletedById = null;
                entity.ModificationDate = DateTime.UtcNow;
                entity.ModifiedById = _currentUserId;
            }

            _dbSet.UpdateRange(entities);
        }

        public virtual void HardRemove(T entity, bool? isOwnerRequired = false)
        {
            ValidateOwner(entity, isOwnerRequired);
            _dbSet.Remove(entity);
        }

        public virtual void HardRemoveRange(List<T> entities, bool? isOwnerRequired = false)
        {
            foreach (var entity in entities) ValidateOwner(entity, isOwnerRequired);

            _dbSet.RemoveRange(entities);
        }

        #region Helper

        private void ValidateOwner(T entity, bool? isOwnerRequired = false)
        {
            if (isOwnerRequired.HasValue && isOwnerRequired.Value && entity.CreatedById.HasValue &&
                entity.CreatedById != _currentUserId)
                throw new UnauthorizedAccessException();
        }

        #endregion
    }
}
