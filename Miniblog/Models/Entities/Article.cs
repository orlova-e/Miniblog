﻿using Miniblog.Models.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Miniblog.Models.Entities
{
    public class Article
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        [Required]
        public string Header { get; set; }
        [Required]
        public string Text { get; set; }
        public bool Visibility { get; set; }
        public bool MenuVisibility { get; set; }
        public EntryType EntryType { get; set; }
        //[Required]
        public string Link { get; set; }
        public string Tags { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public List<Image> Images { get; set; }
        public Guid? TopicId { get; set; }
        public Topic Topic { get; set; }
        //public List<ArticleTag> ArticleTags { get; set; }
        //public Guid? SeriesId { get; set; }
        //public Series Series { get; set; }
        public List<Comment> Comments { get; set; }
        public List<UserBookmark> Bookmarks { get; set; }
        public List<UserFavourite> Likes { get; set; }
        public ArticleOptions DisplayOptions { get; set; }
        public Article()
        {
            Images = new List<Image>();
            Comments = new List<Comment>();
            Bookmarks = new List<UserBookmark>();
            Likes = new List<UserFavourite>();
            //ArticleTags = new List<ArticleTag>();
        }
    }
}