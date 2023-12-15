using System;
using System.Collections.Generic;
using AutoMapper;
using Kimado.Core.Contracts;

namespace BugAnalysis.Api
{
    public class AutoMapperConfig : ICacheService<IMapper>
    {
        private Dictionary<object, IMapper> Mappers = new Dictionary<object, IMapper>();
        public IMapper this[object key] { get => Mappers[key]; set => Mappers[key] = value; }

        public TimeSpan ExpirationDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Add(object key, IMapper entity)
        {
            this[key] = entity;
        }

        public void Cleanup()
        {
            throw new NotImplementedException();
        }

        public IMapper Get(object typeKey)
        {
            return this[typeKey];
        }

        public void Remove(object typeKey)
        {
            throw new NotImplementedException();
        }
    }
}
