namespace Opine.Snapshots
{
    public class Snapshot
    {
        public Snapshot(object id, long version, object snapshotData)
        {
            Id = id;
            Version = version;
            SnapshotData = snapshotData;
        }

        public object Id { get; private set; }
        public long Version { get; private set; }
        public object SnapshotData { get; private set; }
    }
}