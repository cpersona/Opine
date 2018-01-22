using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opine.Snapshots
{
    public class CachedSnapshotWriter : ISnapshotStore
    {
        private List<Snapshot> snapshots = 
            new List<Snapshot>();

        private ISnapshotStore snapshotStore;

        public CachedSnapshotWriter(ISnapshotStore snapshotStore)
        {
            this.snapshotStore = snapshotStore;
        }

        public async Task<Snapshot> Read(Type type, object id)
        {
            return await snapshotStore.Read(type, id);
        }

        public async Task Store(Snapshot snapshot)
        {
            snapshots.Add(snapshot);
        }

        public async Task SaveChanges()
        {
            foreach (var s in snapshots)
            {
                await snapshotStore.Store(s);
            }
        }
    }
}