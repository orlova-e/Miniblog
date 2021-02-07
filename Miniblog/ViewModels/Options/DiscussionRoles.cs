using Domain.Entities;
using Domain.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Options
{
    public class DiscussionRoles
    {
        [Required]
        public RoleType Type { get; set; }
        [Required]
        public string Discriminator { get; set; }
        public bool WriteComments { get; set; }
        public bool ModerateComments { get; set; }

        public static explicit operator DiscussionRoles(Role role)
            => new DiscussionRoles
            {
                Type = role.Type,
                Discriminator = role.Discriminator,
                WriteComments = role.WriteComments,
                ModerateComments = false
            };

        public static explicit operator DiscussionRoles(ExtendedRole extendedRole)
            => new DiscussionRoles
            {
                Type = extendedRole.Type,
                Discriminator = extendedRole.Discriminator,
                WriteComments = extendedRole.WriteComments,
                ModerateComments = extendedRole.ModerateComments
            };

        public static Role operator +(Role role, DiscussionRoles discussionRoles)
        {
            if (role.Type != discussionRoles.Type)
                throw new ArgumentException();
            if (role.Discriminator != discussionRoles.Discriminator)
                throw new ArgumentException();

            role.WriteComments = discussionRoles.WriteComments;

            return role;
        }

        public static ExtendedRole operator +(ExtendedRole extendedRole, DiscussionRoles discussionRoles)
        {
            if (extendedRole.Type != discussionRoles.Type)
                throw new ArgumentException();
            if (extendedRole.Discriminator != discussionRoles.Discriminator)
                throw new ArgumentException();

            extendedRole.WriteComments = discussionRoles.WriteComments;
            extendedRole.ModerateComments = discussionRoles.ModerateComments;

            return extendedRole;
        }
    }
}
