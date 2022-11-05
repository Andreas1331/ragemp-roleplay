using GTANetworkAPI;
using GTARoleplay.Library.Attachments.Data;
using System.Collections.Generic;
using System.Linq;

namespace GTARoleplay.Library.Attachments
{
    public class AttachmentHandler
    {
        private static List<EntityAttachment> allAttachments = new List<EntityAttachment>();

        public static void CreateNewAttachment(Player attachedPlayer, EntityAttachment attachment)
        {
            // Check if an attachment already exists 
            if (allAttachments.Any(x => x.AttachedEntity.Equals(attachment.AttachedEntity)))
                return;
            allAttachments.Add(attachment);
            string attachedAsString = NAPI.Util.ToJson(attachment);
            attachedPlayer.SetSharedData("AttachedContainer", attachedAsString);
        }

        public static void RemoveAttachment(Player attachedPlayer)
        {
            if (allAttachments.Any(x => x.AttachedEntity.Equals(attachedPlayer.Handle.Value)))
            {
                allAttachments.Remove(allAttachments.FirstOrDefault(x => x.AttachedEntity.Equals(attachedPlayer.Handle.Value)));
                attachedPlayer.ResetSharedData("AttachedContainer");
            }
        }
    }
}
