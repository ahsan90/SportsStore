﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contain_All_Products()
        {
            //Arrange- create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"}
            }.AsQueryable);

            //Arrnge -create controller
            AdminController targer = new AdminController(mock.Object);

            //Action
            Product[] result = ((IEnumerable<Product>) targer.Index().ViewData.Model).ToArray();

            //Assert
            Assert.AreEqual(result.Length,3);
            Assert.AreEqual("P1",result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);       
        }

        [TestMethod]
        public void Can_Edit_Products()
        {
            //Arrange- create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "p3"}
            }.AsQueryable());

            //Arrange- create the controller
            AdminController target = new AdminController(mock.Object);
            //Act
            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;

            //Assert
            Assert.AreEqual(1,p1.ProductID);
            Assert.AreEqual(2,p1.ProductID);
            Assert.AreEqual(3, p1.ProductID);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
            }.AsQueryable());

            AdminController target = new AdminController(mock.Object);

            Product result = (Product) target.Edit(4).ViewData.Model;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product{Name = "Test"};
            ActionResult result = target.Edit(product);
            mock.Verify(m=>m.SaveProduct(product));
            Assert.IsNotInstanceOfType(result,typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Change_Invalid_Change()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };
            target.ModelState.AddModelError("error", "error");
            ActionResult result = target.Edit(product);
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            Product prod = new Product{ProductID = 2,Name = "Test"};
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 3, Name = "P3"},
            }.AsQueryable());
            AdminController target = new AdminController(mock.Object);
            target.Delete(prod.ProductID);
            mock.Verify(m=>m.DeleteProduct(prod.ProductID));
        }
    }
}
