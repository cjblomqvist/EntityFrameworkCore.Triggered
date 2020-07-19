﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Transactions
{
    public interface IBeforeRollbackTrigger<in TEntity>
        where TEntity : class
    {
        Task BeforeRollback(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
    }
}
