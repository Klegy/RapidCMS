﻿using System;
using System.Linq.Expressions;

namespace RapidCMS.Common.Data
{
    public class Query : IQuery
    {
        internal LambdaExpression? QueryExpression;

        public static Query Default()
        {
            return new Query
            {
                Skip = 0,
                Take = 1000
            };
        }

        public static Query TakeElements(int take)
        {
            return new Query
            {
                Skip = 0,
                Take = take
            };
        }

        public static Query Create(int pageSize, int pageNumber, string? searchTerm, int? activeTab)
        {
            return new Query
            {
                Skip = pageSize * (pageNumber - 1),
                Take = pageSize,
                SearchTerm = searchTerm,
                ActiveTab = activeTab
            };
        }

        public void HasMoreData(bool hasMoreData)
        {
            MoreDataAvailable = hasMoreData;
        }

        public void SetDataViewExpression(LambdaExpression expression)
        {
            QueryExpression = expression;
        }

        public int? ActiveTab { get; private set; }

        public int Skip { get; private set; }
        public int Take { get; private set; }

        public string? SearchTerm { get; private set; }

        public bool MoreDataAvailable { get; private set; } = false;
    }

    internal class TypedQuery<TEntity> : IQuery, IQuery<TEntity>
        where TEntity : IEntity
    {
        private readonly Query _query;

        public static IQuery<TEntity> Convert(IQuery query)
        {
            if (query is Query q1)
            {
                return new TypedQuery<TEntity>(q1);
            }
            else if (query is TypedQuery<TEntity> q2)
            {
                return q2;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public TypedQuery(Query query)
        {
            _query = query;
        }

        public int Skip => _query.Skip;

        public int Take => _query.Take;

        public string? SearchTerm => _query.SearchTerm;

        public bool MoreDataAvailable => _query.MoreDataAvailable;

        public Expression<Func<TEntity, bool>>? DataViewExpression => _query.QueryExpression as Expression<Func<TEntity, bool>>;

        public void HasMoreData(bool hasMoreData)
        {
            _query.HasMoreData(hasMoreData);
        }
    }
}