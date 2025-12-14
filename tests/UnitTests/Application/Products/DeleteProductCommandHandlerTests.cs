using Moq;
using Xunit;
using ProductOrderAPI.Application.Common.Interfaces;
using ProductOrderAPI.Application.Products.Commands.DeleteProduct;
using ProductOrderAPI.Domain.Entities;
using ProductOrderAPI.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using ProductOrderAPI.UnitTests.Common;

namespace ProductOrderAPI.UnitTests.Application.Products;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;

    public DeleteProductCommandHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
    }

    [Fact]
    public async Task Handle_GivenValidCommand_ShouldDeleteProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var existingProduct = new Product("Test Product", "Description", new Domain.ValueObjects.Money("USD", 10.0m), 5);
        existingProduct.Id = productId;

        var products = new List<Product> { existingProduct }.AsQueryable();

        var mockSet = new Mock<DbSet<Product>>();
        mockSet.As<IQueryable<Product>>().Setup(dbSet => dbSet.Provider).Returns(new TestAsyncQueryProvider<Product>(products.Provider));
        mockSet.As<IQueryable<Product>>().Setup(dbSet => dbSet.Expression).Returns(products.Expression);
        mockSet.As<IQueryable<Product>>().Setup(dbSet => dbSet.ElementType).Returns(products.ElementType);
        mockSet.As<IQueryable<Product>>().Setup(dbSet => dbSet.GetEnumerator()).Returns(products.GetEnumerator());

        mockSet.Setup(dbSet => dbSet.FindAsync(new object[] { productId }, It.IsAny<CancellationToken>()))
               .ReturnsAsync(existingProduct);

        _mockContext.Setup(context => context.Products).Returns(mockSet.Object);
        _mockContext.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new DeleteProductCommandHandler(_mockContext.Object);
        var command = new DeleteProductCommand(productId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockSet.Verify(dbSet => dbSet.Remove(existingProduct), Times.Once);
        _mockContext.Verify(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenNonExistentProductId_ShouldThrowNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        
        var products = new List<Product>().AsQueryable();

        var mockSet = new Mock<DbSet<Product>>();
        mockSet.As<IQueryable<Product>>().Setup(dbSet => dbSet.Provider).Returns(new TestAsyncQueryProvider<Product>(products.Provider));
        mockSet.As<IQueryable<Product>>().Setup(dbSet => dbSet.Expression).Returns(products.Expression);
        mockSet.As<IQueryable<Product>>().Setup(dbSet => dbSet.ElementType).Returns(products.ElementType);
        mockSet.As<IQueryable<Product>>().Setup(dbSet => dbSet.GetEnumerator()).Returns(products.GetEnumerator());

        mockSet.Setup(dbSet => dbSet.FindAsync(new object[] { productId }, It.IsAny<CancellationToken>()))
               .ReturnsAsync((Product)null);

        _mockContext.Setup(context => context.Products).Returns(mockSet.Object);

        var handler = new DeleteProductCommandHandler(_mockContext.Object);
        var command = new DeleteProductCommand(productId);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"Product\" ({productId}) was not found.");
        
        _mockContext.Verify(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
