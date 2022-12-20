using Microsoft.EntityFrameworkCore.Query.Internal;
using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Blocks;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.DataTransferObjects.Texts;
using SavageOrcs.Enums;
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
        private readonly IHelperService _imageService;
        private readonly IRepository<Text> _textRepository;
        private readonly IRepository<Mark> _markRepository;
        private readonly IRepository<Cluster> _clusterRepository;
        private readonly IRepository<TextToCluster> _textsToClustersRepository;
        private readonly IRepository<TextToMark> _textsToMarksRepository;
        private readonly IRepository<Block> _blockRepository;

        public TextService(IUnitOfWork unitOfWork, IRepository<Text> textRepository, IRepository<Block> blockRepository, IHelperService imageService, IRepository<TextToCluster> textsToClustersRepository, IRepository<TextToMark> textsToMarksRepository, IRepository<Cluster> clusterRepository, IRepository<Mark> markRepository) : base(unitOfWork)
        {
            _textRepository = textRepository;
            _blockRepository = blockRepository;
            _imageService = imageService;
            _textsToClustersRepository = textsToClustersRepository;
            _textsToMarksRepository = textsToMarksRepository;
            _clusterRepository = clusterRepository;
            _markRepository = markRepository;
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

            foreach(var textToMark in text.TextsToMarks)
            {
                _textsToMarksRepository.Delete(textToMark);
            }

            foreach (var textToCluster in text.TextsToClusters)
            {
                _textsToClustersRepository.Delete(textToCluster);
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

            if (textSaveDto.UrlDtos is not null)
            {
                foreach(var urlDto in textSaveDto.UrlDtos)
                {
                    if (urlDto.Type == ObjectType.Mark)
                    {
                        var mark = await _markRepository.GetTAsync(x => x.Id == urlDto.Id);

                        if (mark is not null)
                        {
                            var textToMark = new TextToMark
                            {
                                MarkId = mark.Id,
                                TextId = text.Id
                            };

                            await _textsToMarksRepository.AddAsync(textToMark);
                        }
                    }
                    else if (urlDto.Type == ObjectType.Cluster)
                    {
                        var cluster = await _clusterRepository.GetTAsync(x => x.Id == urlDto.Id);

                        if (cluster is not null)
                        {
                            var textToCluster = new TextToCluster
                            {
                                ClusterId = cluster.Id,
                                TextId = text.Id
                            };

                            await _textsToClustersRepository.AddAsync(textToCluster);
                        }
                    }
                }
            }
            await UnitOfWork.SaveChangesAsync();
            return new TextSaveResultDto()
            {
                Success = true,
                Id = text.Id
            };
        }

        public async Task<TextDto[]> GetTextsByFilters(string? textName, string? textSubject, Guid[]? curatorIds)
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

            if (curatorIds is not null && curatorIds.Length > 0)
            {
                texts = texts.Where(x => x.Curator is not null && curatorIds.Contains(x.Curator.Id));
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
