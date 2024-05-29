using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TradexSample.Models.Domain;
using TradexSample.Models.ViewModels;
using TradexSample.Repositories;

namespace TradexSample.Controllers
{
    public class AdminBlogPostController : Controller
    {
        private readonly ITagRepository _tagRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public AdminBlogPostController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
        {
            this._tagRepository = tagRepository;
            this._blogPostRepository = blogPostRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            //get tag from repository
            var tags = await _tagRepository.GetAllAsync();

            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })

            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            //map view model to domain model
            var blogpost = new BlogPost
            {
                Heading = addBlogPostRequest.Heading,
                PageTitle = addBlogPostRequest.PageTitle,
                Content = addBlogPostRequest.Content,
                ShortDescription = addBlogPostRequest.ShortDescription,
                FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible

            };

            //map tags to selected tags
            var selectedTag = new List<Tag>();
            foreach (var item in addBlogPostRequest.SelectedTags)
            {
                var selectedTagIdAsGuid = Guid.Parse(item);
                var existingTag = await _tagRepository.GetAsync(selectedTagIdAsGuid);

                if (existingTag != null)
                {
                    selectedTag.Add(existingTag);
                }
            }
            //mapping tag back to domain model
            blogpost.Tags = selectedTag;

            await _blogPostRepository.AddAsync(blogpost);
            return RedirectToAction("Add");
        }


        [HttpGet]
        public async Task<IActionResult> List()
        {
            var blogposts = await _blogPostRepository.GetAllAsync();
            return View(blogposts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            //retrive the result from the repository

            var blogpost =await _blogPostRepository.GetAsync(id);
            var tagsdomainmodel=await _tagRepository.GetAllAsync();
            //map domain model in view model

            if (blogpost != null)
            {
                var model = new EditBlogPostRequest
                {
                    Id = blogpost.Id,
                    Heading = blogpost.Heading,
                    PageTitle = blogpost.PageTitle,
                    Content = blogpost.Content,
                    Author = blogpost.Author,
                    FeaturedImageUrl = blogpost.FeaturedImageUrl,
                    UrlHandle = blogpost.UrlHandle,
                    ShortDescription = blogpost.ShortDescription,
                    PublishedDate = blogpost.PublishedDate,
                    Visible = blogpost.Visible,
                    Tags = tagsdomainmodel.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }),
                    SelectedTags = blogpost.Tags.Select(x => x.Id.ToString()).ToArray()
                };
                return View(model);
            }

            //pass data to view
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            //map view model to domail model
            var blogpostdomailmodel = new BlogPost
            {
                Id = editBlogPostRequest.Id,
                Heading=editBlogPostRequest.Heading,
                PageTitle=editBlogPostRequest.PageTitle,
                Content = editBlogPostRequest.Content,
                Author = editBlogPostRequest.Author,
                ShortDescription=editBlogPostRequest.ShortDescription,
                FeaturedImageUrl=editBlogPostRequest.FeaturedImageUrl,
                PublishedDate=editBlogPostRequest.PublishedDate,
                UrlHandle=editBlogPostRequest.UrlHandle,
                Visible= editBlogPostRequest.Visible,
            };

            //map tags to domain model

            var selectedtags = new List<Tag>();

            foreach (var st in editBlogPostRequest.SelectedTags)
            {
                if (Guid.TryParse(st, out var tag))
                { 
                    var foundTag=await _tagRepository.GetAsync(tag);
                    if (foundTag != null)
                    {
                        selectedtags.Add(foundTag);
                    }
                }
                

            }

            blogpostdomailmodel.Tags= selectedtags;


            //submit information to repository to update

         var updatedBlog=   await _blogPostRepository.UpdateAsync(blogpostdomailmodel);

            if (updatedBlog != null)
            {
                //show success notification
                return RedirectToAction("Edit");
            }
            return RedirectToAction("Edit");
            //reference to GET
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            //dto to domain
            var deletedBlogPost = await _blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

            if (deletedBlogPost != null)
            {
                return RedirectToAction("List");
            }

            return RedirectToAction("Edit",new { id=editBlogPostRequest.Id});
            //display the response
        }
    }
}
