﻿using MediatR;

namespace Project.Cqrs.Queries
{
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
           where TQuery : IQuery<TResponse>
    {
    }

     
}