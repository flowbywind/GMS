using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMS.Cms.Contract;
using GMS.Cms.DAL;
using GMS.Framework.Utility;
using System.Data.Objects;
using GMS.Framework.Contract;
using EntityFramework.Extensions;
using GMS.Core.Cache;

namespace GMS.Cms.BLL
{
    public class CmsService : ICmsService
    {
        #region Article CURD
        public Article GetArticle(int id)
        {
            using (var dbContext = new CmsDbContext())
            {
                return dbContext.Articles.Include("Tags").FirstOrDefault(a=>a.ID == id);
            }
        }

        public IEnumerable<Article> GetArticleList(ArticleRequest request = null)
        {
            request = request ?? new ArticleRequest();
            using (var dbContext = new CmsDbContext())
            {
                IQueryable<Article> articles = dbContext.Articles.Include("Channel");

                if (!string.IsNullOrEmpty(request.Title))
                    articles = articles.Where(u => u.Title.Contains(request.Title));

                if (request.ChannelId > 0)
                    articles = articles.Where(u => u.ChannelId == request.ChannelId);

                if (request.IsActive != null)
                    articles = articles.Where(u => u.IsActive == request.IsActive);

                return articles.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public void SaveArticle(Article article)
        {
            using (var dbContext = new CmsDbContext())
            {
                var tags = new List<Tag>();

                foreach (var tag in article.Tags)
                {
                    var existTag = dbContext.Tags.FirstOrDefault(t => t.Name == tag.Name);
                    if (existTag != null)
                        existTag.Hits++;
                    tags.Add(existTag ?? tag);
                }

                if (article.ID > 0)
                {
                    article.TagString = string.Empty;
                    dbContext.Update<Article>(article);
                    dbContext.Entry(article).Collection(m => m.Tags).Load();
                    article.Tags = tags;
                    dbContext.SaveChanges();
                }
                else
                {
                    article.Tags = tags;
                    dbContext.Insert<Article>(article);
                }
            }
        }

        public void DeleteArticle(List<int> ids)
        {
            using (var dbContext = new CmsDbContext())
            {
                dbContext.Articles.Include("Tags").Where(u => ids.Contains(u.ID)).ToList().ForEach(a => { a.Tags.Clear(); dbContext.Articles.Remove(a); });
                dbContext.SaveChanges();
            }
        }
        #endregion


        #region Channel CURD
        public Channel GetChannel(int id)
        {
            using (var dbContext = new CmsDbContext())
            {
                return dbContext.Find<Channel>(id);
            }
        }

        public IEnumerable<Channel> GetChannelList(ChannelRequest request = null)
        {
            request = request ?? new ChannelRequest();
            using (var dbContext = new CmsDbContext())
            {
                IQueryable<Channel> channels = dbContext.Channels;

                if (!string.IsNullOrEmpty(request.Name))
                    channels = channels.Where(u => u.Name.Contains(request.Name));

                if (request.IsActive != null)
                    channels = channels.Where(u => u.IsActive == request.IsActive);

                return channels.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public void SaveChannel(Channel channel)
        {
            using (var dbContext = new CmsDbContext())
            {
                if (channel.ID > 0)
                {
                    dbContext.Update<Channel>(channel);
                }
                else
                {
                    dbContext.Insert<Channel>(channel);
                }
            }
        }

        public void DeleteChannel(List<int> ids)
        {
            using (var dbContext = new CmsDbContext())
            {
                dbContext.Channels.Where(u => ids.Contains(u.ID)).Delete();
            }
        }
        #endregion

        public IEnumerable<Tag> GetTagList(TagRequest request = null)
        {
            request = request ?? new TagRequest();
            using (var dbContext = new CmsDbContext())
            {
                IQueryable<Tag> tags = dbContext.Tags;

                if (request.Orderby == Orderby.Hits)
                    return tags.OrderByDescending(u => u.Hits).ToPagedList(request.PageIndex, request.PageSize);
                else
                    return tags.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }
    }
}
