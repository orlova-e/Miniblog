using Domain.Entities;
using Domain.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Options
{
    public class DiscussionRoles
    {
        [Required]
        public string Type { get; set; }
        [Required]
        public string Discriminator { get; set; }
        public bool WriteComments { get; set; }
        public bool ModerateComments { get; set; }

        public static explicit operator DiscussionRoles(Role role)
            => new DiscussionRoles
            {
                Type = Enum.GetName(typeof(RoleType), role.Type),
                Discriminator = role.Discriminator,
                WriteComments = role.WriteComments,
                ModerateComments = false
            };

        public static explicit operator DiscussionRoles(ExtendedRole extendedRole)
            => new DiscussionRoles
            {
                Type = Enum.GetName(typeof(RoleType), extendedRole.Type),
                Discriminator = extendedRole.Discriminator,
                WriteComments = extendedRole.WriteComments,
                ModerateComments = extendedRole.ModerateComments
            };

        public static Role operator +(Role role, DiscussionRoles discussionRoles)
        {
            if (!Enum.GetName(typeof(RoleType), role.Type).Equals(discussionRoles.Type))
                throw new ArgumentException();

            role.WriteComments = discussionRoles.WriteComments;

            return role;
        }

        public static ExtendedRole operator +(ExtendedRole extendedRole, DiscussionRoles discussionRoles)
        {
            if (!Enum.GetName(typeof(RoleType), extendedRole.Type).Equals(discussionRoles.Type))
                throw new ArgumentException();

            extendedRole.WriteComments = discussionRoles.WriteComments;
            extendedRole.ModerateComments = discussionRoles.ModerateComments;

            return extendedRole;
        }
    }

}
