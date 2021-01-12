namespace MicroCore
{
using MassTransit;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class MultipleLenderRequestClient<TRequest> : IRequestClient<TRequest> where TRequest : class
    {
        public IRequestClient<TRequest> Inner { get; }

        public IServiceProvider Provider { get; }

        public MultipleLenderRequestClient(IRequestClient<TRequest> inner, IServiceProvider provider)
        {
            Inner = inner;
            Provider = provider;
        }
        public RequestHandle<TRequest> Create(TRequest message, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
        {
            return Inner.Create(message, cancellationToken, timeout);
        }

        public RequestHandle<TRequest> Create(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
        {
            PreGetResponse(values);
            return Inner.Create(values, cancellationToken, timeout);
        }

        public Task<Response<T>> GetResponse<T>(TRequest message, CancellationToken cancellationToken = default, RequestTimeout timeout = default) where T : class
        {
            PreGetResponse(message);
            return Inner.GetResponse<T>(message, cancellationToken, timeout);
        }

        public Task<Response<T>> GetResponse<T>(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default) where T : class
        {
            PreGetResponse(values);
            return Inner.GetResponse<T>(values, cancellationToken, timeout);
        }

        public Task<(Task<Response<T1>>, Task<Response<T2>>)> GetResponse<T1, T2>(TRequest message, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
        {
            PreGetResponse(message);
            return Inner.GetResponse<T1, T2>(message, cancellationToken, timeout);
        }

        public Task<(Task<Response<T1>>, Task<Response<T2>>)> GetResponse<T1, T2>(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
        {
            PreGetResponse(values);
            return Inner.GetResponse<T1, T2>(values, cancellationToken, timeout);
        }

        private void PreGetResponse(object obj)
        {
            
        }
    }
}