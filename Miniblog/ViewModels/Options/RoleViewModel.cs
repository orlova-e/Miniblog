using Domain.Entities;
using Domain.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Options
{
    public class RoleViewModel
    {
        [Required]
        public RoleType Type { get; set; }
        [Required]
        public string Discriminator { get; set; }
        public bool WriteArticles { get; set; }
        public bool ModerateArticles { get; set; }
        public bool CreateTopics { get; set; }
        public bool ModerateTopics { get; set; }
        public bool CreateTags { get; set; }
        public bool ModerateTags { get; set; }
        public bool OverrideOwnArticle { get; set; }

        public static explicit operator RoleViewModel(Role role)
            => new RoleViewModel
            {
                Type = role.Type,
                Discriminator = role.Discriminator,
                WriteArticles = role.WriteArticles,
                CreateTopics = role.CreateTopics,
                CreateTags = role.CreateTags,
                OverrideOwnArticle = role.OverrideOwnArticle,
                ModerateArticles = false,
                ModerateTopics = false,
                ModerateTags = false
            };

        public static explicit operator RoleViewModel(ExtendedRole extendedRole)
            => new RoleViewModel
            {
                Type = extendedRole.Type,
                Discriminator = extendedRole.Discriminator,
                WriteArticles = extendedRole.WriteArticles,
                CreateTopics = extendedRole.CreateTopics,
                CreateTags = extendedRole.CreateTags,
                OverrideOwnArticle = extendedRole.OverrideOwnArticle,
                ModerateArticles = extendedRole.ModerateArticles,
                ModerateTopics = extendedRole.ModerateTopics,
                ModerateTags = extendedRole.ModerateTags
            };

        public static Role operator +(Role role, RoleViewModel roleViewModel)
        {
            if (role.Type != roleViewModel.Type)
                throw new ArgumentException();
            if (role.Discriminator != roleViewModel.Discriminator)
                throw new ArgumentException();

            role.WriteArticles = roleViewModel.WriteArticles;
            role.CreateTopics = roleViewModel.CreateTopics;
            role.CreateTags = roleViewModel.CreateTags;
            role.OverrideOwnArticle = roleViewModel.OverrideOwnArticle;

            return role;
        }

        public static ExtendedRole operator +(ExtendedRole extendedRole, RoleViewModel roleViewModel)
        {
            if (extendedRole.Type != roleViewModel.Type)
                throw new ArgumentException();
            if (extendedRole.Discriminator != roleViewModel.Discriminator)
                throw new ArgumentException();

            extendedRole.WriteArticles = roleViewModel.WriteArticles;
            extendedRole.CreateTopics = roleViewModel.CreateTopics;
            extendedRole.CreateTags = roleViewModel.CreateTags;
            extendedRole.OverrideOwnArticle = roleViewModel.OverrideOwnArticle;
            extendedRole.ModerateArticles = roleViewModel.ModerateArticles;
            extendedRole.ModerateTopics = roleViewModel.ModerateTopics;
            extendedRole.ModerateTags = roleViewModel.ModerateTags;

            return extendedRole;
        }
    }
}
