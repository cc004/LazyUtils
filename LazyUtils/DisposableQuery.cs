using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LazyUtils;

public sealed class DisposableQuery<T> : IQueryable<T>, IDisposable
{
    private IQueryable<T> query;
    private IDisposable disposable;

    public DisposableQuery(IQueryable<T> query, IDisposable disposable)
    {
        this.query = query;
        this.disposable = disposable;
    }
    public Expression Expression => query.Expression;

    public Type ElementType => query.ElementType;

    public IQueryProvider Provider => query.Provider;

    public void Dispose()
    {
        disposable.Dispose();
    }

    public IEnumerator<T> GetEnumerator() => query.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => query.GetEnumerator();
}