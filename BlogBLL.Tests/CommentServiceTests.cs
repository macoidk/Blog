using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CommentServiceTests : TestBase
    {
        private ICommentService _commentService;
        private IUnitOfWork _unitOfWork;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _fixture = new Fixture();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            
            Rebind<IUnitOfWork>(_unitOfWork);
            _commentService = Kernel.Get<ICommentService>();
            
        }
        

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsComment()
        {
            var comment = _fixture.Build<Comment>().With(c => c.Id, 1).Create();
            _unitOfWork.Comments.GetByIdAsync(1).Returns(Task.FromResult(comment));

            var result = await _commentService.GetByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public void GetByIdAsync_NonExistingId_ThrowsEntityNotFoundException()
        {
            _unitOfWork.Comments.GetByIdAsync(999).Returns(Task.FromResult<Comment>(null));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _commentService.GetByIdAsync(999));
        }
        

        [Test]
        public async Task GetByArticleIdAsync_ExistingArticleId_ReturnsComments()
        {
            var comments = _fixture.CreateMany<Comment>(3).ToList();
            _unitOfWork.Comments.GetByArticleIdAsync(1).Returns(Task.FromResult(comments.AsEnumerable()));

            var result = await _commentService.GetByArticleIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
        }
        

        [Test]
        public async Task GetRootCommentsByArticleIdAsync_ExistingArticleId_ReturnsRootComments()
        {
            var rootComments = _fixture.CreateMany<Comment>(2).ToList();
            _unitOfWork.Comments.GetRootCommentsByArticleIdAsync(1).Returns(Task.FromResult(rootComments.AsEnumerable()));

            var result = await _commentService.GetRootCommentsByArticleIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }
        

        [Test]
        public async Task GetWithRepliesAsync_ExistingId_ReturnsCommentWithReplies()
        {
            var comment = _fixture.Build<Comment>()
                .With(c => c.Id, 1)
                .With(c => c.ChildComments, _fixture.CreateMany<Comment>(2).ToList())
                .Create();
            _unitOfWork.Comments.GetWithRepliesAsync(1).Returns(Task.FromResult(comment));

            var result = await _commentService.GetWithRepliesAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.ChildComments.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetWithRepliesAsync_NonExistingId_ThrowsEntityNotFoundException()
        {
            _unitOfWork.Comments.GetWithRepliesAsync(999).Returns(Task.FromResult<Comment>(null));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _commentService.GetWithRepliesAsync(999));
        }
        

        [Test]
        public async Task GetByUserIdAsync_ExistingUserId_ReturnsComments()
        {
            var comments = _fixture.CreateMany<Comment>(4).ToList();
            _unitOfWork.Comments.GetByUserIdAsync(1).Returns(Task.FromResult(comments.AsEnumerable()));

            var result = await _commentService.GetByUserIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(4));
        }
        

        [Test]
        public void CreateAsync_EmptyContent_ThrowsArgumentException()
        {
            var commentDto = new CommentDto { Content = "" };

            Assert.ThrowsAsync<ArgumentException>(() => _commentService.CreateAsync(commentDto));
        }

       

       

        [Test]
        public async Task UpdateAsync_ValidCommentDto_UpdatesComment()
        {
            var comment = _fixture.Build<Comment>().With(c => c.Id, 1).Create();
            var commentDto = _fixture.Build<CommentDto>().With(c => c.Id, 1).Create();
            _unitOfWork.Comments.GetByIdAsync(1).Returns(Task.FromResult(comment));
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(1));

            await _commentService.UpdateAsync(commentDto);

            Assert.That(comment.Content, Is.EqualTo(commentDto.Content));
            Assert.That(comment.UpdateDate, Is.Not.Null);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Test]
        public void UpdateAsync_NonExistingId_ThrowsEntityNotFoundException()
        {
            var commentDto = _fixture.Create<CommentDto>();
            _unitOfWork.Comments.GetByIdAsync(commentDto.Id).Returns(Task.FromResult<Comment>(null));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _commentService.UpdateAsync(commentDto));
        }

       

       

        [Test]
        public async Task DeleteAsync_ExistingId_DeletesComment()
        {
            var comment = _fixture.Build<Comment>().With(c => c.Id, 1).Create();
            _unitOfWork.Comments.GetByIdAsync(1).Returns(Task.FromResult(comment));
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(1));

            await _commentService.DeleteAsync(1);

            _unitOfWork.Comments.Received(1).Delete(comment);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Test]
        public void DeleteAsync_NonExistingId_ThrowsEntityNotFoundException()
        {
            _unitOfWork.Comments.GetByIdAsync(999).Returns(Task.FromResult<Comment>(null));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _commentService.DeleteAsync(999));
        }

       
    }
}