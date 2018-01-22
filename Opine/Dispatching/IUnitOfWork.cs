using System.Threading.Tasks;

namespace Opine.Dispatching
{
    public interface IUnitOfWork
    {
        Task Commit();
    }
}