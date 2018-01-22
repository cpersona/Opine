using System;

namespace Opine.Messaging
{
    public static class Categories
    {
        public const string Events = "events";
        public const string Commands = "commands";
        public const string Snapshots = "snapshots";
        public const string Failures = "failures";
    }

    public class Stream
    {
        public Stream(string category)
            : this(category, null, null)
        {

        }

        public Stream(string category, Type streamType, object id)
        {
            Category = category;
            StreamType = streamType;
            Id = id;
        }

        public string Category { get; private set; }
        public Type StreamType { get; private set; }
        public object Id { get; private set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //
            
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            var other = obj as Stream;
            // TODO: write your implementation of Equals() here
            return Category.EqualsTo(other.Category)
                && (StreamType == null || StreamType.Equals(other.StreamType))
                && (Id == null || Id.Equals(other.Id));
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return Category.GetHashCode()
                | (StreamType?.GetHashCode() ?? 0)
                | (Id?.GetHashCode() ?? 0);
        }

        public override string ToString()
        {
            var result = Category;
            if (StreamType != null)
            {
                result += string.Format("-{0}-{1}", StreamType.Name, Id);
            }
            return result.ToLower();
        }
    }
}