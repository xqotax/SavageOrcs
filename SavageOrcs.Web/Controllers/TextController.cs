using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavageOrcs.DataTransferObjects.Blocks;
using SavageOrcs.DataTransferObjects.Texts;
using SavageOrcs.Enums;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Constants;
using SavageOrcs.Web.ViewModels.Mark;
using SavageOrcs.Web.ViewModels.Text;
using SavageOrcs.Web.ViewModels.Text.Blocks;

namespace SavageOrcs.Web.Controllers
{
    public class TextController : Controller
    {
        private readonly ITextService _textService;
        private readonly ICuratorService _curatorService;
        private readonly IHelperService _helperService;

        public TextController(ITextService textService, ICuratorService curatorService, IHelperService helperService)
        {
            _textService = textService;
            _curatorService = curatorService;
            _helperService = helperService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Revision(Guid id)
        {

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

        private static string GetHtmlText(BlockDto block)
        {
            if (block.Type == BlockType.Header)
            {
                return "<h" + block.AdditionParameter + ">" + block.Content?.Replace('"', '\'') + "</h" + block.AdditionParameter + ">";
            }
            else if (block.Type == BlockType.Text)
            {
                return "<p>" + block.Content?.Replace('"', '\'') + "</p>";
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
                return "<img class=\"textRevisionImage\" src=\"" + block.Content + "\" title=\"" + block.AdditionParameter?.Replace('"', '\'') + "\"/>";
            }
            else if (block.Type == BlockType.Video)
            {
                return "<video controls class=\"textRevisionVideo\" src=\"" + block.Content + "\" title=\"" + block.AdditionParameter?.Replace('"', '\'') + "\"/>";
            }

            else return "";
        }

        private static string ArrayToLi(string[] items)
        {
            var stringToReturn = "";
            foreach (var item in items)
            {
                stringToReturn += "<li>" + item?.Replace('"', '\'') + "</li>";
            };
            return stringToReturn;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(Guid? id)
        {
            TextDto? textDto = null;
            var emptySelect = _helperService.GetEmptySelect();
            var emptySelectArr = new GuidIdAndNameViewModel[] {
                new GuidIdAndNameViewModel
                {
                    Id = emptySelect.Id,
                    Name = emptySelect.Name
                }
            };

            if (id is not null)
                textDto = await _textService.GetTextById(id.Value);

            var curatorDtos = await _curatorService.GetCurators();
            var ukrTexts = (await _textService.GetTexts()).Where(x => !x.EnglishVersion).ToList();

            var addTextViewModel = new AddTextViewModel()
            {
                Id = textDto?.Id,
                Name = textDto?.Name,
                Subject = textDto?.Subject,
                CuratorId = textDto?.Curator?.Id,
                CuratorName = textDto?.Curator?.Name,
                EnglishVersion = textDto is not null && textDto.EnglishVersion,
                Curators = emptySelectArr.Concat(curatorDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.DisplayName
                })).ToArray(),
                UkrTexts = emptySelectArr.Concat(ukrTexts.Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.Name + x.CreatedDate.ToString("MM/dd/yyyy HH:mm")
                })).ToArray(),
                ToDelete = false,
                IsNew = textDto is null,
                Blocks = null
            };
            if (addTextViewModel.EnglishVersion && textDto?.UkrTextId is not null)
            {
                var ukrText = await _textService.GetTextById(textDto.UkrTextId.Value);
                if (ukrText is not null)
                {
                    addTextViewModel.UkrTextId = ukrText.Id;
                    addTextViewModel.UkrTextName = ukrText.Name + ukrText.CreatedDate.ToString("MM/dd/yyyy HH:mm");
                }
            }


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
                    else if (blockDto.Type == BlockType.Video)
                        addTextViewModel.Blocks.Images.Add(new ImageBlockViewModel
                        {
                            Id = blockDto.CustomId,
                            Src = blockDto.Content,
                            Index = blockDto.Index,
                            Caption = blockDto.AdditionParameter
                        });
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
                UkrTextId = saveTextViewModel?.UkrTextId,
                EnglishVersion = saveTextViewModel is not null && saveTextViewModel.EnglishVersion,
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
                })).Concat(saveTextViewModel.Blocks.Videos.Select(x => new BlockDto
                {
                    CustomId = x.Id,
                    Content = x.Src,
                    Type = BlockType.Video,
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

            var urlDtos = new List<UrlDto> { };
            try
            {
                if (textSaveDto.BlockDtos is not null)
                {
                    foreach (var block in textSaveDto.BlockDtos)
                    {
                        if ((block.Type == BlockType.List || block.Type == BlockType.Text || block.Type == BlockType.Header) && block.Content is not null)
                        {
                            var textToEdit = block.Content;
                            while (textToEdit.Contains("<a href=\""))
                            {
                                var textToFind = textToEdit;

                                var urlDto = _helperService.FindOurUrl(textToFind, out textToEdit);
                                if (urlDto != null)
                                    urlDtos.Add(urlDto);
                            }
                        }
                    }
                }
            }
            catch
            {
                return Json(new SaveMarkResultViewModel
                {
                    Id = null,
                    Success = false,
                    Url = "",
                    Text = "У посиланнях виникла помилка"
                });
            }

            textSaveDto.UrlDtos = urlDtos.ToArray();

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
        public async Task<JsonResult> GetText(Guid id)
        {
            var textDto = await _textService.GetTextById(id);
            var content = textDto.BlockDtos is null ? "" : string.Join("", textDto.BlockDtos.OrderBy(x => x.Index).Select(x => GetHtmlText(x)).ToArray());
            return Json(content);
        }


        [AllowAnonymous]
        public async Task<IActionResult> Catalogue(Guid? textId)
        {
            var unitedTextViewModel = new UnitedCatalogueTextViewModel();
            var textDtos = await _textService.GetShortTexts();
            if (!User.IsInRole("Admin"))
            {
                if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "uk")
                    textDtos = textDtos.Where(x => !x.EnglisVersion).ToArray();
                else
                    textDtos = textDtos.Where(x => x.EnglisVersion).ToArray();
            }

            unitedTextViewModel.Curators = (await _curatorService.GetCurators()).Where(x => x.TextDtos is not null && x.TextDtos.Length > 0)
                .Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id,
                Name =_helperService.GetTranslation(x.DisplayName, x.DisplayNameEng)
            }).ToArray();

            unitedTextViewModel.Texts = textDtos.Select(x => new TextRevisionViewModel
            {
                Id = x.Id,
                Name = x.Name,
                CuratorName = _helperService.GetTranslation(x.Curator?.Name,x.Curator?.NameEng),
                CreatedDate = x.CreatedDate
            }).OrderByDescending(x => x.CreatedDate).ToArray();

            if (textId.HasValue)
            {
                var firstText = unitedTextViewModel.Texts.FirstOrDefault(x => x.Id == textId.Value);
                var textList = unitedTextViewModel.Texts.ToList();

                if (firstText != null)
                {
                    textList.Remove(firstText);
                    textList.Insert(0, firstText);
                    unitedTextViewModel.Texts = textList.ToArray();
                }
            }

            unitedTextViewModel.TextNames = textDtos.Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id,
                Name = x.Name,
            }).ToArray();

            return View(unitedTextViewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> GetTexts([FromBody] UnitedCatalogueTextViewModel filters)
        {
            var textDtos = await _textService.GetTextsByFilters(filters.TextIds, filters.CuratorIds);

            if (!User.IsInRole("Admin"))
            {
                if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "uk")
                    textDtos = textDtos.Where(x => !x.EnglisVersion).ToArray();
                else
                    textDtos = textDtos.Where(x => x.EnglisVersion).ToArray();
            }

            var textCatalogueViewModel = textDtos.Select(x => new TextRevisionViewModel
            {
                Id = x.Id,
                Subject = x.Subject,
                Name = x.Name,
                CuratorId = x.Curator?.Id,
                CuratorName = _helperService.GetTranslation(x.Curator?.Name,x.Curator?.NameEng),
                CreatedDate = x.CreatedDate
            }).OrderByDescending(x => x.CreatedDate).ToArray();

            return PartialView("_CatalogueDataRows", textCatalogueViewModel);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult AddImage()
        {
            return PartialView("_AddImage");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddVideo()
        {
            return PartialView("_AddVideo");
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            TextDto? textDto = null;

            if (id is not null)
                textDto = await _textService.GetTextById(id.Value);

            var curatorDtos = await _curatorService.GetCurators();
            var ukrTexts = (await _textService.GetTexts()).Where(x => !x.EnglishVersion).ToList();

            var addTextViewModel = new AddTextViewModel()
            {
                Id = textDto?.Id,
                Name = textDto?.Name,
                Subject = textDto?.Subject,
                CuratorId = textDto?.Curator?.Id,
                CuratorName = textDto?.Curator?.Name,
                EnglishVersion = textDto is not null && textDto.EnglishVersion,
                UkrTexts = ukrTexts.Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.Name + x.CreatedDate.ToString("MM/dd/yyyy HH:mm")
                }).ToArray(),
                Curators = curatorDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.DisplayName
                }).ToArray(),
                ToDelete = true,
                IsNew = textDto is null,
                Blocks = null

            };
            if (addTextViewModel.EnglishVersion && textDto?.UkrTextId is not null)
            {
                var ukrText = await _textService.GetTextById(textDto.UkrTextId.Value);
                if (ukrText is not null)
                {
                    addTextViewModel.UkrTextId = ukrText.Id;
                    addTextViewModel.UkrTextName = ukrText.Name + ukrText.CreatedDate.ToString("MM/dd/yyyy HH:mm");
                }
            }

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
                    else if (blockDto.Type == BlockType.Video)
                        addTextViewModel.Blocks.Images.Add(new ImageBlockViewModel
                        {
                            Id = blockDto.CustomId,
                            Src = blockDto.Content,
                            Index = blockDto.Index,
                            Caption = blockDto.AdditionParameter
                        });
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
            var result = await _textService.DeleteText(id);

            return Json(new ResultViewModel
            {
                Id = id,
                Success = result,
                Url = "/Text/Catalogue",
                Text = result ? "Текст успішно видалено" : "Помилка, зверніться до адміністратора"
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ImageToInsert([FromBody] string content)
        {
            return PartialView("_ImageToInsert", new StringAndStringViewModel
            {
                Name1 = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss fff"),
                Name2 = content
            });
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult VideoToInsert([FromBody] string content)
        {
            return PartialView("_VideoToInsert", new StringAndStringViewModel
            {
                Name1 = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss fff"),
                Name2 = content
            });
        }

    }
}
