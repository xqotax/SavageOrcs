using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;
using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.DataTransferObjects.Blocks;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.DataTransferObjects.Texts;
using SavageOrcs.Enums;
using SavageOrcs.Services;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Cluster;
using SavageOrcs.Web.ViewModels.Constants;
using SavageOrcs.Web.ViewModels.Mark;
using SavageOrcs.Web.ViewModels.Text;
using SavageOrcs.Web.ViewModels.Text.Blocks;
using System.Globalization;

namespace SavageOrcs.Web.Controllers
{
    public class TextController : Controller
    {
        private readonly ITextService _textService;
        private readonly ICuratorService _curatorService;

        public TextController(ITextService textService, ICuratorService curatorService)
        {
            _textService = textService;
            _curatorService = curatorService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Revision(Guid id) { 

            var textDto = await _textService.GetTextById(id);

            if (textDto == null)
                return NotFound();

            var textRevisionViewModel = new TextRevisionViewModel
            {
                Id = textDto.Id,
                Subject = textDto.Subject,
                Name = textDto.Name,
                Content = textDto.BlockDtos is null ? "" : String.Join("", textDto.BlockDtos.OrderBy(x => x.Index).Select(x => GetHtmlText(x)).ToArray()),
                CuratorId = textDto.Curator?.Id,
                CuratorName = textDto.Curator?.Name
            };

            return View(textRevisionViewModel);
        }

        private string GetHtmlText(BlockDto block)
        {
            if (block.Type == BlockType.Header)
            {
                return "<h" + block.AdditionParameter + ">" + block.Content + "</h" + block.AdditionParameter + ">";
            }
            else if (block.Type == BlockType.Text)
            {
                return "<p>" + block.Content + "</p>";
            }
            else if (block.Type == BlockType.List)
            {
                var items = block.Content?.Split("\n_\n");
                if (items is null)
                    return "";
                if (block.AdditionParameter == "ordered")
                {
                    return "<ol class=\"number-list\">" + ArrayToLi(items) + "</ol>";
                }
                else
                    return "<ul class=\"circle-list\">" + ArrayToLi(items) + "</ul>";
            }
            else if (block.Type == BlockType.Raw)
            {
                return "<div class=\"row\">" + block.Content + "</div>";
            }
            else if (block.Type == BlockType.Image)
            {
                return "<img class=\"textRevisionImage\" src=\"" + block.Content +"\" title=\"" + block.AdditionParameter +  "\"/>";
            }

            else return "";
        }

        private string ArrayToLi(string[] items)
        {
            var stringToReturn = "";
            foreach(var item in items)
            {
                stringToReturn += "<li>" + item + "</li>";
            };
            return stringToReturn;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(Guid? id)
        {
            TextDto? textDto = null;

            if (id is not null)
                textDto = await _textService.GetTextById(id.Value);

            var curatorDtos = await _curatorService.GetCurators();

            var addTextViewModel = new AddTextViewModel()
            {
                Id = textDto?.Id,
                Name = textDto?.Name,
                Subject = textDto?.Subject,
                CuratorId = textDto?.Curator?.Id,
                CuratorName = textDto?.Curator?.Name,
                Curators = curatorDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.DisplayName
                }).ToArray(),
                ToDelete = false,
                IsNew = textDto is null,
                Blocks = null
                
            };
            if (textDto?.BlockDtos is not null)
            {
                addTextViewModel.Blocks = new TextBlockViewModel();
                foreach (var blockDto in textDto.BlockDtos)
                {
                    if (blockDto.Type == BlockType.Text)
                        addTextViewModel.Blocks.Paragraphs.Add(new ParagraphBlockViewModel
                        {
                            Id = blockDto.CustomId,
                            Text = blockDto.Content,
                            Index = blockDto.Index,
                        });
                    else if (blockDto.Type == BlockType.Image)
                        addTextViewModel.Blocks.Images.Add(new ImageBlockViewModel
                        {
                            Id = blockDto.CustomId,
                            Src = blockDto.Content,
                            Index = blockDto.Index,
                            Caption = blockDto.AdditionParameter
                        });
                    //else if (blockDto.Type == BlockType.CheckList)
                    //    addTextViewModel.Blocks.CheckBoxes.Add(new CheckListBlockViewModel
                    //    {
                    //        Id = blockDto.CustomId,
                    //        Items = blockDto.Content?.Split("\n_\n"),
                    //        Index = blockDto.Index,
                    //    });
                    else if (blockDto.Type == BlockType.List)
                        addTextViewModel.Blocks.Listes.Add(new ListBlockViewModel
                        {
                            Id = blockDto.CustomId,
                            Items = blockDto.Content?.Split("\n_\n"),
                            Index = blockDto.Index,
                            Style = blockDto.AdditionParameter
                        });
                    else if (blockDto.Type == BlockType.Header)
                        addTextViewModel.Blocks.Headers.Add(new HeaderBlockViewModel
                        {
                            Id = blockDto.CustomId,
                            Text = blockDto.Content,
                            Index = blockDto.Index,
                            Level = Convert.ToInt32(blockDto.AdditionParameter)
                        });
                    else if (blockDto.Type == BlockType.Header)
                        addTextViewModel.Blocks.Raws.Add(new RawBlockViewModel
                        {
                            Id = blockDto.CustomId,
                            Text = blockDto.Content,
                            Index = blockDto.Index,
                        });
                }
            }
            return View(addTextViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> SaveText([FromBody] AddTextViewModel saveTextViewModel)
        {
            var textSaveDto = new TextSaveDto
            {
                Id = saveTextViewModel?.Id,
                Subject = saveTextViewModel?.Subject,
                Name = saveTextViewModel?.Name,
                CuratorId = saveTextViewModel?.CuratorId,
                BlockDtos = saveTextViewModel?.Blocks?.Headers.Select(x => new BlockDto
                {
                    CustomId = x.Id,
                    Content = x.Text,
                    Type = BlockType.Header,
                    Index = x.Index,
                    AdditionParameter = x.Level.ToString()
                })//.Concat(saveTextViewModel.Blocks.CheckBoxes.Select(x => new BlockDto
                //{
                //    CustomId = x.Id,
                //    Type = BlockType.CheckList,
                //    Index = x.Index,
                //    Content = x.Items is not null ? String.Join("\n_\n", x.Items) : null
                //}))
                .Concat(saveTextViewModel.Blocks.Listes.Select(x => new BlockDto
                {
                    CustomId = x.Id,
                    Content = x.Items is not null ? String.Join("\n_\n", x.Items) : null,
                    Type = BlockType.List,
                    Index = x.Index,
                    AdditionParameter = x.Style
                })).Concat(saveTextViewModel.Blocks.Raws.Select(x => new BlockDto
                {
                    CustomId = x.Id,
                    Content = x.Text,
                    Type = BlockType.Raw,
                    Index = x.Index
                })).Concat(saveTextViewModel.Blocks.Images.Select(x => new BlockDto
                {
                    CustomId = x.Id,
                    Content = x.Src,
                    Type = BlockType.Image,
                    Index = x.Index,
                    AdditionParameter = x.Caption
                })).Concat(saveTextViewModel.Blocks.Paragraphs.Select(x => new BlockDto
                {
                    CustomId = x.Id,
                    Content = x.Text,
                    Type = BlockType.Text,
                    Index = x.Index
                })).ToArray()
            };

            var result = await _textService.SaveText(textSaveDto);

            return Json(new SaveMarkResultViewModel
            {
                Id = result.Id,
                Success = result.Success,
                Url = "/Text/Revision/{id}",
                Text = "Текст успішно збережений"
            });
        }


        [AllowAnonymous]
        public async Task<IActionResult> Catalogue()
        {
            var curatorDtos = await _curatorService.GetCurators();

            var filterCatalogueTextViewModel = new FilterCatalogueTextViewModel
            {
                CuratorName = "",
                TextName = "",
                TextSubject = "",
                Curators = curatorDtos.Select(x => x.DisplayName).ToArray()
            };

            return View(filterCatalogueTextViewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> GetTexts([FromBody] FilterCatalogueTextViewModel filters)
        {
            var textDtos = await _textService.GetTextsByFilters(filters.TextName, filters.TextSubject, filters.CuratorName);

            var textCatalogueViewModel = textDtos.Select(x => new TextCatalogueViewModel { 
                Id = x.Id,
                Subject = x.Subject,
                Name = x.Name,
                Curator = x.Curator is null ? null: new GuidIdAndNameViewModel { Id = x.Curator.Id, Name = x.Curator.Name} 
            }).ToArray();

            return Json(textCatalogueViewModel);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult AddImage()
        {
            return PartialView("_AddImage");
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            TextDto? textDto = null;

            if (id is not null)
                textDto = await _textService.GetTextById(id.Value);

            var curatorDtos = await _curatorService.GetCurators();

            var addTextViewModel = new AddTextViewModel()
            {
                Id = textDto?.Id,
                Name = textDto?.Name,
                Subject = textDto?.Subject,
                CuratorId = textDto?.Curator?.Id,
                CuratorName = textDto?.Curator?.Name,
                Curators = curatorDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.DisplayName
                }).ToArray(),
                ToDelete = true,
                IsNew = textDto is null,
                Blocks = null

            };
            if (textDto?.BlockDtos is not null)
            {
                addTextViewModel.Blocks = new TextBlockViewModel();
                foreach (var blockDto in textDto.BlockDtos)
                {
                    if (blockDto.Type == BlockType.Text)
                        addTextViewModel.Blocks.Paragraphs.Add(new ParagraphBlockViewModel
                        {
                            Id = blockDto.CustomId,
                            Text = blockDto.Content,
                            Index = blockDto.Index,
                        });
                    else if (blockDto.Type == BlockType.Image)
                        addTextViewModel.Blocks.Images.Add(new ImageBlockViewModel
                        {
                            Id = blockDto.CustomId,
                            Src = blockDto.Content,
                            Index = blockDto.Index,
                            Caption = blockDto.AdditionParameter
                        });
                    //else if (blockDto.Type == BlockType.CheckList)
                    //    addTextViewModel.Blocks.CheckBoxes.Add(new CheckListBlockViewModel
                    //    {
                    //        Id = blockDto.CustomId,
                    //        Items = blockDto.Content?.Split("\n_\n"),
                    //        Index = blockDto.Index,
                    //    });
                    else if (blockDto.Type == BlockType.List)
                        addTextViewModel.Blocks.Listes.Add(new ListBlockViewModel
                        {
                            Id = blockDto.CustomId,
                            Items = blockDto.Content?.Split("\n_\n"),
                            Index = blockDto.Index,
                            Style = blockDto.AdditionParameter
                        });
                    else if (blockDto.Type == BlockType.Header)
                        addTextViewModel.Blocks.Headers.Add(new HeaderBlockViewModel
                        {
                            Id = blockDto.CustomId,
                            Text = blockDto.Content,
                            Index = blockDto.Index,
                            Level = Convert.ToInt32(blockDto.AdditionParameter)
                        });
                    else if (blockDto.Type == BlockType.Header)
                        addTextViewModel.Blocks.Raws.Add(new RawBlockViewModel
                        {
                            Id = blockDto.CustomId,
                            Text = blockDto.Content,
                            Index = blockDto.Index,
                        });
                }
            }
            return View("Add", addTextViewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteText()
        {
            return PartialView("_Delete");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> DeleteConfirm(Guid id)
        {
            var result = await _textService.DeleteCluster(id);

            return Json(new ResultViewModel
            {
                Id = id,
                Success = result,
                Url = "/Text/Catalogue",
                Text = result ? "Текст успішно видалено" : "Помилка, зверніться до адміністратора"
            });
        }

    }
}
