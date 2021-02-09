namespace Repo.Interfaces
{
    public interface IPlainEntityRepository<T> : IPlainRepository<T>, IWorkingWithRange<T>
        where T : class
    { }
}
