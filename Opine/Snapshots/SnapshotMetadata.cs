namespace Opine.Snapshots
{
    public class SnapshotMetadata
    {
        public SnapshotMetadata(long version)
        {
            Version = version;
        }

        public long Version { get; private set; }
    }
}