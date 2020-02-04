using JetBrains.ReSharper.Plugins.Unity.Yaml.Psi.Resolve;
using JetBrains.ReSharper.Psi;
using JetBrains.Serialization;

namespace JetBrains.ReSharper.Plugins.Unity.Yaml.Psi.Caches.AssetMethods
{
    public class AssetMethodData
    {
        public string OwnerId { get;}
        public string MethodName { get; }
        public EventHandlerArgumentMode Mode { get; }
        public string Type { get; }
        public AssetDocumentReference AssetDocumentReference { get; }

        public AssetMethodData(string ownerId, string methodName, EventHandlerArgumentMode mode, string type, AssetDocumentReference assetDocumentReference)
        {
            OwnerId = ownerId;
            MethodName = methodName;
            Mode = mode;
            Type = type;
            AssetDocumentReference = assetDocumentReference;
        }

        public void WriteTo(UnsafeWriter writer)
        {
            writer.Write(OwnerId);
            writer.Write(MethodName);
            writer.Write((int)Mode);
            writer.Write(Type);
            AssetDocumentReference.WriteTo(writer);
        }

        public static AssetMethodData ReadFrom(UnsafeReader reader)
        {
            return new AssetMethodData(reader.ReadString(), reader.ReadString(), (EventHandlerArgumentMode)reader.ReadInt32(),
                reader.ReadString(), AssetDocumentReference.ReadFrom(reader));
        }

        protected bool Equals(AssetMethodData other)
        {
            return Equals(OwnerId, other.OwnerId) && MethodName == other.MethodName
                                                  && Mode == other.Mode
                                                  && Type == other.Type
                                                  && Equals(AssetDocumentReference, other.AssetDocumentReference);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(AssetMethodData)) return false;
            return Equals((AssetMethodData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = OwnerId.GetHashCode();
                hashCode = (hashCode * 397) ^ MethodName.GetHashCode() ;
                hashCode = (hashCode * 397) ^ (int) Mode;
                hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ AssetDocumentReference.GetHashCode();
                return hashCode;
            }
        }
    }
}