﻿using ChatMe.Web.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using ChatMe.BussinessLogic.Services.Abstract;
using AutoMapper;
using ChatMe.BussinessLogic.DTO;
using System.Threading.Tasks;

namespace ChatMe.Web.Controllers
{
    [RoutePrefix("api/posts")]
    public class PostsController : Controller
    {
            private IPostService postService;

            public PostsController(IPostService postService) {
                this.postService = postService;
            }

        

        [HttpGet]
        [Route("{userId}")]
        public ActionResult GetAll(string userId, int startIndex = 0, int count = 0) {
            var postsData = postService.GetChunk(userId, User.Identity.GetUserId(), startIndex, count);
            Mapper.Initialize(cfg => cfg.CreateMap<PostDTO, PostViewModel>()
                .ForMember("AvatarUrl", opt => opt.MapFrom(p =>
                   Url.RouteUrl("Avatar", new { userId = p.AuthorId }))
                )
                .ForMember("AuthorLink", opt => opt.MapFrom(p =>
                    Url.RouteUrl("UserProfile", new { userName = p.AuthorUserName }))
                )
            );

            var posts = Mapper.Map<IEnumerable<PostViewModel>>(postsData);

            return Json(posts.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("{userId}/{postId}")]
        public ActionResult Get(string userId, int postId) {
            var postData = postService.Get(userId, User.Identity.GetUserId(), postId);

            Mapper.Initialize(cfg => cfg.CreateMap<PostDTO, PostViewModel>()
                .ForMember("AvatarUrl", opt => opt.MapFrom(p =>
                    Url.RouteUrl("Avatar", new { userId = p.AuthorId }))
                )
                .ForMember("AuthorLink", opt => opt.MapFrom(p =>
                    Url.RouteUrl("UserProfile", new { userName = p.AuthorUserName }))
                )
            );

            return Json(Mapper.Map<PostViewModel>(postData), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("{userId}")]
        public async Task Post(string userId, NewPostViewModel postModel) {
            var newPostData = new NewPostDTO {
                Body = postModel.Body,
                UserId = userId
            };

            await postService.Create(newPostData);
        }

        [HttpPut]
        [Route("{postId}")]
        public async Task Put(int postId, NewPostViewModel postModel) {
            var newPostData = new NewPostDTO {
                Body = postModel.Body
            };

            await postService.Update(newPostData, postId);
        }

        [HttpDelete]
        [Route("{postId}")]
        public async Task Delete(int postId) {
            await postService.Delete(postId);
        }
    }
}
