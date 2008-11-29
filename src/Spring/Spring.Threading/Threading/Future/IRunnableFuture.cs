//namespace Spring.Threading.Future
//{
//    /// <summary> 
//    /// A <see cref="Spring.Threading.Future.IFuture{T}"/> that is a <see cref="Spring.Threading.IRunnable"/>. Successful execution of
//    /// the <see cref="Spring.Threading.IRunnable.Run()"/> method causes completion of the 
//    /// <see cref="Spring.Threading.Future.IFuture{T}"/> and allows access to its results.
//    /// </summary>
//    /// <remarks>
//    /// Sets this <see cref="Spring.Threading.Future.IFuture{T}"/> to the result of its computation
//    /// unless it has been cancelled.
//    /// </remarks>
//    /// <seealso cref="Spring.Threading.Future.FutureTask{T}"/>
//    /// <seealso cref="Spring.Threading.IExecutor"/>
//    /// <author>Doug Lea</author>
//    /// <author>Griffin Caprio (.NET)</author>
//    public interface IRunnableFuture<T> : IRunnable, IFuture<T>
//    {
//    }
//}