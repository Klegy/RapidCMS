﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RapidCMS.Common.Data;
using TestLibrary.Data;
using TestLibrary.Entities;

namespace TestLibrary.Repositories
{
    public class CountryRepository : BaseStructRepository<int, int, CountryEntity>
    {
        private readonly TestDbContext _dbContext;

        public CountryRepository(TestDbContext dbContext, SemaphoreSlim semaphoreSlim) : base(semaphoreSlim)
        {
            _dbContext = dbContext;
        }

        public override async Task DeleteAsync(int id, int? parentId)
        {
            var entry = new CountryEntity { _Id = id };
            _dbContext.Countries.Remove(entry);
            await _dbContext.SaveChangesAsync();
        }

        public override async Task<IEnumerable<CountryEntity>> GetAllAsync(int? parentId, IQuery query)
        {
            var data = await _dbContext.Countries
                .OrderBy(x => x._Id)
                .Skip(query.Skip)
                .Take(query.Take + 1)
                .AsNoTracking()
                .ToListAsync();

            query.HasMoreData(data.Count > query.Take);

            return data.Take(query.Take);
        }

        public override async Task<IEnumerable<CountryEntity>?> GetAllRelatedAsync(IEntity relatedEntity, IQuery query)
        {
            if (relatedEntity is PersonEntity person)
            {
                var data = await _dbContext.Countries
                    .Where(x => x.Persons.Any(x => x.PersonId == person._Id))
                    .OrderBy(x => x._Id)
                    .Skip(query.Skip)
                    .Take(query.Take + 1)
                    .AsNoTracking()
                    .ToListAsync();

                query.HasMoreData(data.Count > query.Take);

                return data.Take(query.Take);
            }

            return null;
        }
        public override async Task<IEnumerable<CountryEntity>?> GetAllNonRelatedAsync(IEntity relatedEntity, IQuery query)
        {
            if (relatedEntity is PersonEntity person)
            {
                var data = await _dbContext.Countries
                    .Where(x => !x.Persons.Any(x => x.PersonId == person._Id))
                    .OrderBy(x => x._Id)
                    .Skip(query.Skip)
                    .Take(query.Take + 1)
                    .AsNoTracking()
                    .ToListAsync();

                query.HasMoreData(data.Count > query.Take);

                return data.Take(query.Take);
            }

            return null;
        }

        public override async Task<CountryEntity> GetByIdAsync(int id, int? parentId)
        {
            return await _dbContext.Countries.AsNoTracking().FirstOrDefaultAsync(x => x._Id == id);
        }

        public override async Task<CountryEntity> InsertAsync(int? parentId, CountryEntity entity, IRelationContainer? relations)
        {
            var entry = _dbContext.Countries.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entry.Entity;
        }

        public override Task<CountryEntity> NewAsync(int? parentId, Type? variantType = null)
        {
            return Task.FromResult(new CountryEntity());
        }

        public override int ParseKey(string id)
        {
            return int.Parse(id);
        }

        public override int? ParseParentKey(string? parentId)
        {
            return int.TryParse(parentId, out var id) ? id : default(int?);
        }

        public override async Task AddAsync(IEntity relatedEntity, int id)
        {
            if (relatedEntity is PersonEntity person)
            {
                var entry = new PersonCountryEntity { CountryId = id, PersonId = person._Id };
                _dbContext.PersonContries.Add(entry);
                await _dbContext.SaveChangesAsync();
            }
        }

        public override async Task RemoveAsync(IEntity relatedEntity, int id)
        {
            if (relatedEntity is PersonEntity person)
            {
                var entry = await _dbContext.PersonContries.FirstOrDefaultAsync(x => x.CountryId == id && x.PersonId == person._Id);
                if (entry != null)
                {
                    _dbContext.PersonContries.Remove(entry);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public override async Task UpdateAsync(int id, int? parentId, CountryEntity entity, IRelationContainer? relations)
        {
            var dbEntity = await _dbContext.Countries.FirstOrDefaultAsync(x => x._Id == id);

            dbEntity.Name = entity.Name;

            _dbContext.Countries.Update(dbEntity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
