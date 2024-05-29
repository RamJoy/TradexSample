using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradexSample.Data;
using TradexSample.Models.Domain;
using TradexSample.Models.ViewModels;
using TradexSample.Repositories;

namespace TradexSample.Controllers
{
    public class AdminTagsController : Controller
    {
        private readonly ITagRepository _tagRepository;

        public AdminTagsController(ITagRepository tagRepository)
        {
            this._tagRepository = tagRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> Add(AddTagRequest addtagrequest)
        {
            var tag = new Tag
            {
                Name = addtagrequest.Name,
                DisplayName = addtagrequest.DisplayName
            };
            //await _dbContext.Tags.AddAsync(tag);
            //await _dbContext.SaveChangesAsync();
            await _tagRepository.AddAsync(tag);

            return RedirectToAction("List");
        }

        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult>  List()
        {

            var tags=await _tagRepository.GetAllAsync();
            return View(tags);
        }

        [HttpGet]
        public async Task<IActionResult>  Edit(Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);
            if (tag != null)
            {
                var edittagrequest = new EditTagRequest
                {
                    Id=tag.Id,
                    Name=tag.Name,
                    DisplayName=tag.DisplayName
                };
                return View(edittagrequest);
            }
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest edittagrequest)
        {
            var tag = new Tag
            {
                Id= edittagrequest.Id,
                Name=edittagrequest.Name,
                DisplayName=edittagrequest.DisplayName
            };

            var updatedTag=await _tagRepository.UpdateAsync(tag);
            if (updatedTag != null)
            {
                //show success notification
            }
            else
            {
                //show error
            }
            //show error notify
            return RedirectToAction("Edit", new { id=edittagrequest.Id});


            
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest edittagrequest)
        {
            var deletedTag= await _tagRepository.DeleteAsync(edittagrequest.Id);

            if (deletedTag != null) 
            {
                return RedirectToAction("List");
            }

            return RedirectToAction("Edit", new { id = edittagrequest.Id });
        }
    }
}
