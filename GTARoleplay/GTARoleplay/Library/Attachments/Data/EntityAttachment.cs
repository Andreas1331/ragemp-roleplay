using GTANetworkAPI;

namespace GTARoleplay.Library.Attachments.Data
{
    public struct EntityAttachment
    {
        public int AttachedEntity { get; }
        public int AttachedToEntity { get; }
        public EntityType AttachedToType { get; }
        public string BoneName { get; }
        public Vector3 Offset { get; }
        public Vector3 Rotation { get; }
        public bool P9 { get; } 
        public bool UseSoftPinning { get; }
        public bool Collision { get; }
        public bool IsPed { get; }
        public int VertexIndex { get; }
        public bool FixedRotation { get; }

        public EntityAttachment(Player attachedEntity, Entity attachedToEntity, string boneName, Vector3 offset, Vector3 rot, bool useSoft, bool collision, bool isPed, int vertexIndex, bool fixedRot)
        {
            this.AttachedEntity = attachedEntity.Handle.Value;
            this.AttachedToEntity = attachedToEntity.Handle.Value;
            this.AttachedToType = attachedToEntity.Type;
            this.BoneName = boneName;
            this.Offset = offset;
            this.Rotation = rot;
            this.UseSoftPinning = useSoft;
            this.Collision = collision;
            this.IsPed = isPed;
            this.VertexIndex = vertexIndex;
            this.FixedRotation = fixedRot;
            this.P9 = true;
        }
    }
}
