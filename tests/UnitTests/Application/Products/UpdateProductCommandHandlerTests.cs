using Moq;
using Xunit;
using ProductOrderAPI.Application.Common.Interfaces;
using ProductOrderAPI.Application.Products.Commands.UpdateProduct;
using ProductOrderAPI.Domain.Entities;
using ProductOrderAPI.Domain.ValueObjects;
using ProductOrderAPI.Application.Products.DTOs;
using ProductOrderAPI.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FluentAssertions;
using ProductOrderAPI.Application.Common.Mappings;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using ProductOrderAPI.UnitTests.Common;

namespace ProductOrderAPI.UnitTests.Application.Products;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        var mapperConfig = new MapperConfiguration(config =>
        {
            config.AddProfile<MappingProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task Handle_GivenValidCommand_ShouldUpdateProductAndReturnDto()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var existingProduct = new Product("Old Name", "Old Description", new Money("USD", 10.0m), 5);
        existingProduct.Id = productId; // Manually set the ID since the constructor doesn't take it
        
        var products = new List<Product> { existingProduct }.AsQueryable();

        var mockSet = new Mock<DbSet<Product>>();
        mockSet.As<IQueryable<Product>>().Setup(dbSet => dbSet.Provider).Returns(new TestAsyncQueryProvider<Product>(products.Provider));
        mockSet.As<IQueryable<Product>>().Setup(dbSet => dbSet.Expression).Returns(products.Expression);
        mockSet.As<IQueryable<Product>>().Setup(dbSet => dbSet.ElementType).Returns(products.ElementType);
        mockSet.As<IQueryable<Product>>().Setup(dbSet => dbSet.GetEnumerator()).Returns(products.GetEnumerator());

        mockSet.Setup(dbSet => dbSet.FindAsync(productId)).ReturnsAsync(existingProduct);

        _mockContext.Setup(context => context.Products).Returns(mockSet.Object);
        _mockContext.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateProductCommandHandler(_mockContext.Object, _mapper);

        var command = new UpdateProductCommand(productId, "New Name", "New Description", new Money("EUR", 15.0m), 10);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
        result.Name.Should().Be(command.Name);
        result.Description.Should().Be(command.Description);
        result.Price.Amount.Should().Be(command.Price.Amount);
        result.Price.Currency.Should().Be(command.Price.Currency);
        result.StockQuantity.Should().Be(command.StockQuantity);

        existingProduct.Name.Should().Be(command.Name);
        existingProduct.Description.Should().Be(command.Description);
        existingProduct.Price.Amount.Should().Be(command.Price.Amount);
        existingProduct.Price.Currency.Should().Be(command.Price.Currency);
        existingProduct.StockQuantity.Should().Be(command.StockQuantity);
        existingProduct.LastModifiedAt.Should().NotBeNull();

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

        _mockContext.Setup(context => context.Products).Returns(mockSet.Object);

        var handler = new UpdateProductCommandHandler(_mockContext.Object, _mapper);

        var command = new UpdateProductCommand(productId, "Name", "Description", new Money("USD", 10.0m), 5);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"Product\" ({productId}) was not found.");
        
        _mockContext.Verify(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
