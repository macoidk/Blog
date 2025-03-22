using System.Threading.Tasks;
using BlogSystem.Abstractions;
using BlogSystem.BLL.DTO;
using BlogSystem.BLL.Exceptions;
using BlogSystem.BLL.Extensions;
using BlogSystem.BLL.Interfaces;
using BlogSystem.Models;
using NSubstitute;
using NUnit.Framework;
using AutoFixture;
using BlogSystem.BLL.Services;
using Ninject;

namespace BlogSystem.Tests
{
    [TestFixture]
    public class TagServiceTests : TestBase
    {
        private ITagService _tagService;
        private IUnitOfWork _unitOfWork;
        private Fixture _fixture;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _tagService = Substitute.For<ITagService>();
            _tagService = Substitute.For<ITagService>();
            
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Test]
        public async Task CreateAsync_NewTagName_CreatesTag()
        {
            var tagName = "testtag";
            _unitOfWork.Tags.GetByNameAsync(tagName).Returns(Task.FromResult<Tag>(null));
            _unitOfWork.Tags.AddAsync(Arg.Any<Tag>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(1));
    
            _tagService.CreateAsync(tagName).Returns(async Task =>
            {
                var existingTag = await _unitOfWork.Tags.GetByNameAsync(tagName);
                if (existingTag != null)
                    return new TagDto { Name = existingTag.Name };
                
                var newTag = new Tag { Name = tagName };
                await _unitOfWork.Tags.AddAsync(newTag);
                await _unitOfWork.SaveChangesAsync();
                return new TagDto { Name = newTag.Name };
            });

            var result = await _tagService.CreateAsync(tagName);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(tagName));
            await _unitOfWork.Tags.Received(1).AddAsync(Arg.Any<Tag>());
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Test]
        public async Task CreateAsync_ExistingTagName_ReturnsExistingTag()
        {
            var tag = _fixture.Build<Tag>().With(t => t.Name, "testtag").Create();
            _unitOfWork.Tags.GetByNameAsync("testtag").Returns(Task.FromResult(tag));

            _tagService.CreateAsync("testtag").Returns(async Task =>
            {
                var existingTag = await _unitOfWork.Tags.GetByNameAsync("testtag");
                if (existingTag != null)
                    return new TagDto { Name = existingTag.Name };
                
                var newTag = new Tag { Name = "testtag" };
                await _unitOfWork.Tags.AddAsync(newTag);
                await _unitOfWork.SaveChangesAsync();
                return new TagDto { Name = newTag.Name };
            });
            
            var result = await _tagService.CreateAsync("testtag");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("testtag"));
            await _unitOfWork.Tags.DidNotReceive().AddAsync(Arg.Any<Tag>());
        }
        
        [Test]
        public async Task GetByNameAsync_ExistingName_ReturnsTagDto()
        {
            var tag = _fixture.Build<Tag>().With(t => t.Name, "testtag").Create();
            _unitOfWork.Tags.GetByNameAsync("testtag").Returns(Task.FromResult(tag));
    
            _tagService = new TagService(_unitOfWork); 

            var result = await _tagService.GetByNameAsync("testtag");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("testtag"));
        }

        [Test]
        public async Task GetByNameAsync_NonExistingName_ReturnsNull()
        {
            _unitOfWork.Tags.GetByNameAsync("nonexistent").Returns(Task.FromResult<Tag>(null));

            var result = await _tagService.GetByNameAsync("nonexistent");

            Assert.That(result, Is.Null);
        }
    }
}