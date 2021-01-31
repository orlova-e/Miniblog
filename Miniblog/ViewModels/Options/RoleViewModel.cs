using Domain.Entities;
using Domain.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Options
{
    public class RoleViewModel
    {
        [Required]
        public string Type { get; set; }
        [Required]
        public string Discriminator { get; set; }
        public bool WriteArticles { get; set; }
        public bool CreateTopics { get; set; }
        public bool ModerateTopics { get; set; }
        public bool CreateTags { get; set; }
        public bool ModerateTags { get; set; }
        public bool OverrideOwnArticle { get; set; }

        public static explicit operator RoleViewModel(Role role)
            => new RoleViewModel
            {
                Type = Enum.GetName(typeof(RoleType), role.Type),
                Discriminator = role.Discriminator,
                WriteArticles = role.WriteArticles,
                CreateTopics = role.CreateTopics,
                CreateTags = role.CreateTags,
                OverrideOwnArticle = role.OverrideOwnArticle,
                ModerateTopics = false,
                ModerateTags = false
            };

        public static explicit operator RoleViewModel(ExtendedRole extendedRole)
            => new RoleViewModel
            {
                Type = Enum.GetName(typeof(RoleType), extendedRole.Type),
                Discriminator = extendedRole.Discriminator,
                WriteArticles = extendedRole.WriteArticles,
                CreateTopics = extendedRole.CreateTopics,
                CreateTags = extendedRole.CreateTags,
                OverrideOwnArticle = extendedRole.OverrideOwnArticle,
                ModerateTopics = extendedRole.ModerateTopics,
                ModerateTags = extendedRole.ModerateTags
            };

        public static Role operator +(Role role, RoleViewModel rolesViewModel)
        {
            if (!Enum.GetName(typeof(RoleType), role.Type).Equals(rolesViewModel.Type))
                throw new ArgumentException();

            role.WriteArticles = rolesViewModel.WriteArticles;
            role.CreateTopics = rolesViewModel.CreateTopics;
            role.CreateTags = rolesViewModel.CreateTags;
            role.OverrideOwnArticle = rolesViewModel.OverrideOwnArticle;

            return role;
        }

        public static ExtendedRole operator +(ExtendedRole extendedRole, RoleViewModel rolesViewModel)
        {
            if (!Enum.GetName(typeof(RoleType), extendedRole.Type).Equals(rolesViewModel.Type))
                throw new ArgumentException();

            extendedRole.WriteArticles = rolesViewModel.WriteArticles;
            extendedRole.CreateTopics = rolesViewModel.CreateTopics;
            extendedRole.CreateTags = rolesViewModel.CreateTags;
            extendedRole.OverrideOwnArticle = rolesViewModel.OverrideOwnArticle;
            extendedRole.ModerateTopics = rolesViewModel.ModerateTopics;
            extendedRole.ModerateTags = rolesViewModel.ModerateTags;

            return extendedRole;
        }
    }
}
