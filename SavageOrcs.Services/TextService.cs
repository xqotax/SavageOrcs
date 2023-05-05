using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Blocks;
using SavageOrcs.DataTransferObjects.Cluster;
using SavageOrcs.DataTransferObjects.Texts;
using SavageOrcs.Enums;
using SavageOrcs.Repositories.Interfaces;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;
using System.Linq;
using Text = SavageOrcs.BusinessObjects.Text;

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

            return texts.Select(x => CreateTextDto(x)).OrderByDescending(x => x.CreatedDate).ToArray();
        }

        public async Task<TextShortDto[]> GetShortTexts()
        {
            var texts = await _textRepository.GetAllAsync();
            return texts.Select(x => new TextShortDto {
                Id = x.Id,
                Name = x.Name,
                Subject = x.Subject,
                CreatedDate = x.CreatedDate,
                EnglisVersion = x.EnglishVersion,
                Curator = x.CuratorId is not null ? 
                    new GuidIdAndStringNameWithEnglishName 
                    { 
                        Id = x.CuratorId.Value, 
                        Name = x.Curator?.Name, 
                        NameEng = x.Curator?.NameEng, 
                    } : null,
            }).OrderByDescending(x => x.CreatedDate).ToArray();
        }
        public async Task<TextDto> GetTextById(Guid id)
        {
            var text = await _textRepository.GetTAsync(x => x.Id == id);
            text ??= new Text();

            return CreateTextDto(text);
        }

        public async Task<TextShortDto[]> GetTextsByCuratorIds(Guid curatorId)
        {
            var texts = await _textRepository.GetAllAsync();

            return texts.Where(x => x.CuratorId.HasValue && x.CuratorId == curatorId)
                .Select(x => new TextShortDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Subject = x.Subject,
                    CreatedDate = x.CreatedDate,
                    EnglisVersion = x.EnglishVersion
                }).ToArray();

        }

        private TextDto CreateTextDto(Text text)
        {
            return new TextDto
            {
                Id = text.Id,
                Name = text.Name,
                Subject = text.Subject,
                EnglishVersion = text.EnglishVersion,
                UkrTextId = text.UkrTextId,
                CreatedDate = text.CreatedDate,
                Curator = text.CuratorId is not null?  new GuidIdAndStringNameWithEnglishName { 
                    Id = text.CuratorId.Value, 
                    Name = text.Curator?.Name, 
                    NameEng = text.Curator?.NameEng
                }:null,
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

            if (textSaveDto.CuratorId == _Constants.EmptySelect)
                text.CuratorId = null;
            else
                text.CuratorId = textSaveDto.CuratorId;

            text.EnglishVersion = textSaveDto.EnglishVersion;

            if (textSaveDto.UkrTextId == _Constants.EmptySelect)
                text.UkrTextId = null;
            else
                text.UkrTextId = textSaveDto.UkrTextId;

            foreach (var block in text.Blocks)
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

        public async Task<TextShortDto[]> GetTextsByFilters(Guid[]? textIds, Guid[]? curatorIds)
        {

            var texts = await _textRepository.GetAllAsync();

            if (textIds is not null && textIds.Length > 0)
            {
                texts = texts.Where(x => textIds.Contains(x.Id));
            }

            if (curatorIds is not null && curatorIds.Length > 0)
            {
                texts = texts.Where(x => x.CuratorId.HasValue && curatorIds.Contains(x.CuratorId.Value));
            }

            return texts.Select(x => new TextShortDto
            {
                Id = x.Id,
                Name = x.Name,
                Subject = x.Subject,
                CreatedDate = x.CreatedDate,
                EnglisVersion = x.EnglishVersion,
                Curator = x.Curator is null? null : new GuidIdAndStringNameWithEnglishName
                {
                    Id = x.Curator.Id,
                    Name = x.Curator.Name,
                    NameEng = x.Curator?.NameEng
                } 
            }).OrderByDescending(x => x.CreatedDate).ToArray(); ;

        }

        public async Task<bool> DeleteText(Guid id)
        {
            var text = await _textRepository.GetTAsync(x => x.Id == id);
            var possibleEngTexts = await _textRepository.GetAllAsync(x => x.UkrTextId.HasValue && x.UkrTextId == id);

            foreach(var possibleEngText in possibleEngTexts)
            {
                possibleEngText.UkrTextId = null;
            }

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
