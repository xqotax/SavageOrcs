using Microsoft.EntityFrameworkCore.Query.Internal;
using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Blocks;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.DataTransferObjects.Texts;
using SavageOrcs.Repositories.Interfaces;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.Services
{
    public class TextService : UnitOfWorkService, ITextService
    {
        private readonly IImageService _imageService;
        private readonly IRepository<Text> _textRepository;
        private readonly IRepository<Block> _blockRepository;

        public TextService(IUnitOfWork unitOfWork, IRepository<Text> textRepository, IRepository<Block> blockRepository, IImageService imageService) : base(unitOfWork)
        {
            _textRepository = textRepository;
            _blockRepository = blockRepository;
            _imageService = imageService;
        }

        public async Task<TextDto[]> GetTexts()
        {
            var texts = await _textRepository.GetAllAsync();

            return texts.Select(x => CreateTextDto(x)).ToArray();
        }

        public async Task<TextDto> GetTextById(Guid id)
        {
            var text = await _textRepository.GetTAsync(x => x.Id == id);

            text ??= new Text();

            return CreateTextDto(text);
        }

        private TextDto CreateTextDto(Text text)
        {
            return new TextDto
            {
                Id = text.Id,
                Name = text.Name,
                Subject = text.Subject,
                Curator = text.CuratorId is not null?  new GuidIdAndStringName { Id = text.CuratorId.Value, Name = text.Curator?.Name }:null,
                BlockDtos = text.Blocks.Select(x => new BlockDto
                {
                    Id = x.Id,
                    CustomId = x.CustomId,
                    Type = x.Type,
                    Content = x.Content is not null ? _imageService.GetStringForText(x.Content) : null,
                    Index = x.Index,
                    AdditionParameter = x.AdditionalParameter
                }).ToArray()
            };
        }

        public async Task<TextSaveResultDto> SaveText(TextSaveDto textSaveDto)
        {
            var text = new Text();

            if (textSaveDto.Id is not null)
            {
                text = await _textRepository.GetTAsync(x => x.Id == textSaveDto.Id);
                text ??= new Text();
            }
            else
            {
                text.Id = Guid.NewGuid();
                text.CreatedDate = DateTime.Now;
                await _textRepository.AddAsync(text);
            }

            text.UpdatedDate = DateTime.Now;
            text.Name = textSaveDto.Name;
            text.Subject = textSaveDto.Subject;
            text.CuratorId = textSaveDto.CuratorId;

            foreach(var block in text.Blocks)
            {
                _blockRepository.Delete(block);
            }

            if (textSaveDto.BlockDtos is not null) {
                foreach (var newBlock in textSaveDto.BlockDtos)
                {
                    var block = new Block
                    {
                        Id = Guid.NewGuid(),
                        Index = newBlock.Index,
                        CustomId = newBlock.CustomId,
                        TextId = text.Id,
                        Type = newBlock.Type,
                        AdditionalParameter = newBlock.AdditionParameter,
                        Content = newBlock.Content is null? null : _imageService.GetBytesForText(newBlock.Content)
                    };

                    await _blockRepository.AddAsync(block);
                }
            }
            await UnitOfWork.SaveChangesAsync();
            return new TextSaveResultDto()
            {
                Success = true,
                Id = text.Id
            };
        }

        public async Task<TextDto[]> GetTextsByFilters(string? textName, string? textSubject, string? curatorName)
        {

            var texts = await _textRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(textName))
            {
                texts = texts.Where(x => x.Name is not null && x.Name.Contains(textName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(textSubject))
            {
                texts = texts.Where(x => x.Subject is not null && x.Subject.Contains(textSubject, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(curatorName))
            {
                texts = texts.Where(x => x.Curator is not null && x.Curator.Name is not null && x.Curator.Name.Contains(curatorName, StringComparison.OrdinalIgnoreCase));
            }

            return texts.Select(x => CreateTextDto(x)).ToArray();

        }

        public async Task<bool> DeleteCluster(Guid id)
        {
            var text = await _textRepository.GetTAsync(x => x.Id == id);

            if (text == null)
                return false;

            foreach (var block in text.Blocks)
            {
                _blockRepository.Delete(block);
            }

            _textRepository.Delete(text);

            await UnitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
