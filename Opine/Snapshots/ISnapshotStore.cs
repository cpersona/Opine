using System;
using System.Threading.Tasks;

namespace Opine.Snapshots
{
    public interface ISnapshotStore
    {
         Task<Snapshot> Read(Type type, object id);
         Task Store(Snapshot snapshot);
    }
}