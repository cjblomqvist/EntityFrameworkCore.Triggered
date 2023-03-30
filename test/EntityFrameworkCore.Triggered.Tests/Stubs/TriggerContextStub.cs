using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerContextStub<TEntity> : ITriggerContext<TEntity>
        where TEntity : class

    {
        public ChangeType ChangeType { get; set; }
        public TEntity Entity { get; set; }
        public TEntity UnmodifiedEntity { get; set; }
        public IDictionary<object, object> Items { get; set; }

        public EntityEntry EntityEntry { get; }
    }
}
